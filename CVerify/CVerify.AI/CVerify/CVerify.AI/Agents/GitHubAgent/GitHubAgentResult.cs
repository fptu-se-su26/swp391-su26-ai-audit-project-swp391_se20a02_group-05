namespace CVerify.AI.Agents.GitHubAgent;

public record GitHubAgentResult(
    object[] RepoAnalyses,
    float OverallActivityScore,
    float OverallContributionScore);
