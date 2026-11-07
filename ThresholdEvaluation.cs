public sealed record LabeledDecision(double Probability, bool WasUrgent);

public static class ThresholdEvaluation
{
    public static (int MissedUrgent, int FalseAlarms, int SentToReview) Evaluate(
        IReadOnlyList<LabeledDecision> samples,
        double urgentThreshold,
        double reviewThreshold)
    {
        int missed = samples.Count(
            s => s.WasUrgent && s.Probability < reviewThreshold);

        int falseAlarms = samples.Count(
            s => !s.WasUrgent && s.Probability >= urgentThreshold);

        int review = samples.Count(
            s => s.Probability >= reviewThreshold
                && s.Probability < urgentThreshold);

        return (missed, falseAlarms, review);
    }
}
