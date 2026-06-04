namespace CVerify.AI.Orchestrators;

public interface ICvAnalysisOrchestrator
{
    Task<object> OrchestrateAsync(Guid submissionId, CancellationToken cancellationToken = default);
}
