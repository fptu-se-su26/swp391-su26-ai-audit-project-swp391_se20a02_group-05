namespace CVerify.AI.Scoring;

public interface IPercentileService
{
    Task<int> GetPercentileAsync(float score);
}
