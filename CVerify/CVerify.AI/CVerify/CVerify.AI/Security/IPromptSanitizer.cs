namespace CVerify.AI.Security;

public interface IPromptSanitizer
{
    SanitizationResult Sanitize(string input);
}

public record SanitizationResult(
    string SafeVersion,
    bool IsSuspicious,
    string[] Reasons);
