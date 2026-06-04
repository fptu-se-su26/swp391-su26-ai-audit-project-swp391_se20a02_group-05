namespace CVerify.AI.Orchestrators;

public interface IJobMatchingOrchestrator
{
    Task<object[]> OrchestrateAsync(Guid candidateId, CancellationToken cancellationToken = default);
}
