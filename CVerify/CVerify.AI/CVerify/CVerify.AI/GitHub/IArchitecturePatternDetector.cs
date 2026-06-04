namespace CVerify.AI.GitHub;

public interface IArchitecturePatternDetector
{
    Task<string[]> DetectAsync(object repoStructure, string codeSnippets);
}
