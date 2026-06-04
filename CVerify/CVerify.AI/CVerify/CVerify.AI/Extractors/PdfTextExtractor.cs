namespace CVerify.AI.Extractors;

public interface ITextExtractor
{
    Task<string> ExtractAsync(byte[] fileContent, CancellationToken cancellationToken = default);
}

public class PdfTextExtractor : ITextExtractor
{
    public async Task<string> ExtractAsync(byte[] fileContent, CancellationToken cancellationToken = default)
    {
        // Implementation will go here
        return string.Empty;
    }
}
