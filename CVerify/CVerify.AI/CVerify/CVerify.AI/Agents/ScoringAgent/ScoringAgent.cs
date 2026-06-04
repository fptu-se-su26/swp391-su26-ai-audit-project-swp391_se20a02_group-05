namespace CVerify.AI.Agents.ScoringAgent;

public record ScoringInput(
    object VerifiedProfile,
    object CvData,
    object GitHubData);

public record ScoredProfile(
    float CompositeScore,
    Dictionary<string, float> Breakdown,
    int Percentile);

public class ScoringAgent : IAgent
{
    private readonly IKernel _kernel;

    public ScoringAgent(IKernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<object> ExecuteAsync(object input, CancellationToken cancellationToken = default)
    {
        if (input is not ScoringInput scoringInput)
            throw new ArgumentException("Invalid input type for ScoringAgent");

        // Implementation will go here
        return new ScoredProfile(0f, new(), 0);
    }
}
