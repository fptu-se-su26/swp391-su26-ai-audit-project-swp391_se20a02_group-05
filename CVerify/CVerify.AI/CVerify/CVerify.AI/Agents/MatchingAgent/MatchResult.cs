namespace CVerify.AI.Agents.MatchingAgent;

public record MatchResult(
    Guid JobId,
    Guid CandidateId,
    float OverallScore,
    float SkillMatchScore,
    float ExperienceScore,
    string[] Strengths,
    string[] Gaps,
    string Explanation);
