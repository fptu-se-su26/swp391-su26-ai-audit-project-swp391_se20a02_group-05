namespace CVerify.AI.Prompts;

public interface IPromptFactory
{
    string GetSystemPrompt();
    string GetUserPrompt(object input);
}

public class CvPromptFactory : IPromptFactory
{
    public string GetSystemPrompt()
    {
        // Implementation will go here
        return string.Empty;
    }

    public string GetUserPrompt(object input)
    {
        // Implementation will go here
        return string.Empty;
    }
}
