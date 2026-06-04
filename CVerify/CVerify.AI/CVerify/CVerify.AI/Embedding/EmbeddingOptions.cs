namespace CVerify.AI.Embedding;

public record EmbeddingOptions(
    string ApiKey,
    string Model,
    int Dimensions);
