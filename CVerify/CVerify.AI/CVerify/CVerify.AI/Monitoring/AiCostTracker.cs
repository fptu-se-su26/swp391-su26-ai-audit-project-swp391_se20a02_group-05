namespace CVerify.AI.Monitoring;

public interface IAiCostTracker
{
    void Record(object activity, decimal cost);
    Task<decimal> GetTotalCostAsync(Guid candidateId);
}

public class AiCostTracker : IAiCostTracker
{
    public void Record(object activity, decimal cost)
    {
        // Implementation will go here - track cost per request
    }

    public async Task<decimal> GetTotalCostAsync(Guid candidateId)
    {
        // Implementation will go here
        return 0m;
    }
}
