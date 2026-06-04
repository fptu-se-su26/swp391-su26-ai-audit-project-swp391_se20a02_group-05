namespace CVerify.AI.GitHub;

public interface ICodeSampler
{
    Task<CodeSample> SampleAsync(
        object repo,
        string token,
        CodeSamplingOptions options,
        CancellationToken cancellationToken = default);
}

public record CodeSample(
    string[] FileContent,
    string[] FileNames);

public record CodeSamplingOptions(
    int MaxFiles,
    int MaxLinesPerFile,
    string[] Extensions);

public class CodeSampler : ICodeSampler
{
    public async Task<CodeSample> SampleAsync(
        object repo,
        string token,
        CodeSamplingOptions options,
        CancellationToken cancellationToken = default)
    {
        // Implementation will go here
        return new(Array.Empty<string>(), Array.Empty<string>());
    }
}
