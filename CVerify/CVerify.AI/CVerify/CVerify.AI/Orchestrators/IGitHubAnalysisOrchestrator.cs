namespace CVerify.AI.Orchestrators;

public interface IGitHubAnalysisOrchestrator
{
    Task<object> OrchestrateAsync(Guid candidateId, string encryptedToken, CancellationToken cancellationToken = default);
}
