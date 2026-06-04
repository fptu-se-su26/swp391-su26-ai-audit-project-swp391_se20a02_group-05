namespace CVerify.AI.Agents.VerificationAgent;

public record VerificationInput(
    Guid CandidateId,
    object RepoAnalyses,
    string[] CvSkills);
