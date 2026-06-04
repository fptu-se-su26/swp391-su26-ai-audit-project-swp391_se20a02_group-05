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
