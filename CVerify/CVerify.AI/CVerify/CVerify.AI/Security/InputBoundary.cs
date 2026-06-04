namespace CVerify.AI.Security;

public class InputBoundary
{
    private const int MaxTokens = 8000;
    private const int MaxCvText = 50000;

    public void ValidateTokenCount(string text)
    {
        // Implementation will go here - enforce token limits
    }

    public void ValidateCvSize(string text)
    {
        if (text.Length > MaxCvText)
            throw new InvalidOperationException($"CV text exceeds maximum size of {MaxCvText} characters");
    }
}
