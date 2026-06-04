namespace CVerify.AI.Agents.GitHubAgent;

public record GitHubAgentInput(Guid CandidateId, string EncryptedToken);

public class GitHubAgent : IAgent
{
    private readonly IKernel _kernel;

    public GitHubAgent(IKernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<object> ExecuteAsync(object input, CancellationToken cancellationToken = default)
    {
        if (input is not GitHubAgentInput githubInput)
            throw new ArgumentException("Invalid input type for GitHubAgent");

        // Implementation will go here
        return new GitHubAgentResult(Array.Empty<object>(), 0f, 0f);
    }
}
