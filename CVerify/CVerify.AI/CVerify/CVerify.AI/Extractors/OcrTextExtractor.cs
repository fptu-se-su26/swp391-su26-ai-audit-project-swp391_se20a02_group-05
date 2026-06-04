namespace CVerify.AI.Extractors;

public class OcrTextExtractor : ITextExtractor
{
    public async Task<string> ExtractAsync(byte[] fileContent, CancellationToken cancellationToken = default)
    {
        // Implementation will go here - Tesseract/Azure DI
        return string.Empty;
    }
}
