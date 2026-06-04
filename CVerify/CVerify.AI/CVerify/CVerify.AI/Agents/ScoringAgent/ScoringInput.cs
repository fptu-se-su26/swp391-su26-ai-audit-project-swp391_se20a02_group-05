namespace CVerify.AI.Agents.ScoringAgent;

public record ScoringInput(
    object VerifiedProfile,
    object CvData,
    object GitHubData);
