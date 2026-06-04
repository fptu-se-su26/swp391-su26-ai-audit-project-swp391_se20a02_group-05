namespace CVerify.AI.Security;

public interface IPromptSanitizer
{
    SanitizationResult Sanitize(string input);
}

public record SanitizationResult(
    string SafeVersion,
    bool IsSuspicious,
    string[] Reasons);

public class PromptSanitizer : IPromptSanitizer
{
    public SanitizationResult Sanitize(string input)
    {
        // Implementation will go here - detect prompt injection patterns
        return new(input, false, Array.Empty<string>());
    }
}
