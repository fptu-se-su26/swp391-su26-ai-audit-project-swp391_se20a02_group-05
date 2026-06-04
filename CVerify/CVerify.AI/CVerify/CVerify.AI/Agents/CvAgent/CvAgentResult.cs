namespace CVerify.AI.Agents.CvAgent;

public record CvAgentResult(
    string[] Skills,
    string[] Experience,
    string[] Education,
    float CompletenessScore,
    object RawSections);
