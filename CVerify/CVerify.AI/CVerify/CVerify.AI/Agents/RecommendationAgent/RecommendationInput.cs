namespace CVerify.AI.Agents.RecommendationAgent;

public record RecommendationInput(
    Guid CandidateId,
    object ScoredProfile,
    object[] Matches,
    object CvData);
