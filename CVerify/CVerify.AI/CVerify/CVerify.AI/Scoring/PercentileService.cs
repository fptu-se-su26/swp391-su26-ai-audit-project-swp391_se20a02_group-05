namespace CVerify.AI.Scoring;

public interface IPercentileService
{
    Task<int> GetPercentileAsync(float score);
}

public class PercentileService : IPercentileService
{
    public async Task<int> GetPercentileAsync(float score)
    {
        // Implementation will go here - calculate percentile rank
        return 0;
    }
}
