namespace CVerify.AI.Extractors;

public interface ITextExtractor
{
    Task<string> ExtractAsync(byte[] fileContent, CancellationToken cancellationToken = default);
}
