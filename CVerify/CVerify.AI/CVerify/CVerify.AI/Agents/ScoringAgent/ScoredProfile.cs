namespace CVerify.AI.Agents.ScoringAgent;

public record ScoredProfile(
    float CompositeScore,
    Dictionary<string, float> Breakdown,
    int Percentile);
