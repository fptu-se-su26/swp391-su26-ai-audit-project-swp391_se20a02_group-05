namespace CVerify.AI.Agents.RecommendationAgent;

public record RecommendationReport(
    string[] CvImprovements,
    string[] SkillGaps,
    string[] LearningPaths,
    object[] JobMatchExplanations);
