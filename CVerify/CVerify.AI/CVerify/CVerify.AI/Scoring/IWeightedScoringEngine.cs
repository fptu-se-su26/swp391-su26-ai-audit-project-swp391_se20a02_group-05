namespace CVerify.AI.Scoring;

public interface IWeightedScoringEngine
{
    float CalculateCompositeScore(Dictionary<string, float> dimensions, Dictionary<string, float> weights);
}
