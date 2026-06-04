namespace CVerify.AI.Embedding;

public interface IEmbeddingService
{
    Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken = default);
}

public record EmbeddingOptions(
    string ApiKey,
    string Model,
    int Dimensions);

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingOptions _options;

    public OpenAiEmbeddingService(EmbeddingOptions options)
    {
        _options = options;
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken = default)
    {
        // Implementation will go here - call OpenAI embedding API
        return Array.Empty<float>();
    }
}
