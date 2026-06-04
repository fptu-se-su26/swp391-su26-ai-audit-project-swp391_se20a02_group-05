namespace CVerify.AI.Monitoring;

public interface IAiCostTracker
{
    void Record(object activity, decimal cost);
    Task<decimal> GetTotalCostAsync(Guid candidateId);
}
