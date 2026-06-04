namespace CVerify.AI.Agents.CvAgent;

public record CvAgentInput(Guid SubmissionId, string RawText);

public record CvAgentResult(
    string[] Skills,
    string[] Experience,
    string[] Education,
    float CompletenessScore,
    object RawSections);

public class CvAgent : IAgent
{
    private readonly IKernel _kernel;

    public CvAgent(IKernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<object> ExecuteAsync(object input, CancellationToken cancellationToken = default)
    {
        if (input is not CvAgentInput cvInput)
            throw new ArgumentException("Invalid input type for CvAgent");

        // Implementation will go here
        return new CvAgentResult([], [], [], 0f, new());
    }
}
