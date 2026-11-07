#pragma warning disable OPENAI001

using System.ClientModel;
using OpenAI.Decisions;

string apiKey =
    Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException(
        "OPENAI_API_KEY is required.");

DecisionsClient client = new(apiKey);

DecisionCreationOptions options = new()
{
    Model = "gpt-6-luna",
    InputItems =
    {
        new DecisionsUserMessageItem
        {
            Content =
            {
                new DecisionsInputTextContentPart(
                    """
                    Checkout is failing for every customer.
                    The first error appeared 18 minutes ago.
                    No successful orders have been recorded since.
                    """)
            }
        }
    },
    Questions =
    {
        new DecisionsPredicateQuestion
        {
            Name = "requires_urgent_response",
            Instructions =
            {
                new DecisionsInputTextInstructionsPart
                {
                    Text =
                        """
                        This incident is actively blocking a
                        revenue-producing customer workflow
                        and requires immediate human response.
                        """
                }
            }
        }
    }
};

double? probability = null;

try
{
    DecisionResource result =
        await client.CreateDecisionAsync(options);

    // A question can return a refusal instead of a predicate answer.
    probability =
        (result.Answers[0] as DecisionsPredicateAnswer)?.Probability;
}
catch (ClientResultException failure)
{
    Console.Error.WriteLine(
        $"Decisions call failed with status {failure.Status}.");
}

string route = probability switch
{
    null    => "human-review",
    >= 0.85 => "urgent-response",
    >= 0.35 => "human-review",
    _       => "normal-queue"
};

Console.WriteLine(
    $"Route: {route}; probability: {probability?.ToString("P1") ?? "none"}");
