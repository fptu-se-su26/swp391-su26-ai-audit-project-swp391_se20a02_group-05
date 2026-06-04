namespace CVerify.AI.Agents.MatchingAgent;

public record MatchingInput(
    Guid CandidateId,
    object ScoredProfile,
    object[] Jobs);
