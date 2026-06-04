namespace CVerify.AI.Prompts;

public interface IPromptFactory
{
    string GetSystemPrompt();
    string GetUserPrompt(object input);
}
