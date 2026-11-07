# OpenAI Decisions Routing

A small .NET 10 console app that uses `DecisionsClient` from the [OpenAI .NET library](https://www.nuget.org/packages/OpenAI/2.15.0) to route an incident. The model returns a probability, and the application's own policy decides where the incident goes.

| Result | Route |
| --- | --- |
| Probability at or above 0.85 | `urgent-response` |
| Probability from 0.35 up to 0.85 | `human-review` |
| Probability below 0.35 | `normal-queue` |
| Refusal or failed call | `human-review` |

The thresholds are illustrative. Choose yours from labeled examples and the cost of false positives and false negatives in your own domain. The sample keeps them in the switch so it's easy to read, and `incident-routing.example.json` shows how they could look in versioned configuration.

`ThresholdEvaluation.cs` counts missed urgent incidents, false alarms and review volume for a pair of thresholds, using probabilities you stored alongside the real outcomes.

## Requirements

- .NET 10 SDK
- An OpenAI API key with access to `gpt-6-luna`

## Run

```bash
export OPENAI_API_KEY="your-key"
dotnet run
```

On PowerShell, use `$env:OPENAI_API_KEY = "your-key"` instead of `export`.

## Status

This sample builds against `OpenAI` 2.15.0, but it hasn't been run against the live API yet. The .NET Decisions API is experimental under `OPENAI001` and may change.

A probability is not authorization. Keep deterministic policy, human review or both between the answer and any irreversible action.

## License

[MIT](LICENSE)
