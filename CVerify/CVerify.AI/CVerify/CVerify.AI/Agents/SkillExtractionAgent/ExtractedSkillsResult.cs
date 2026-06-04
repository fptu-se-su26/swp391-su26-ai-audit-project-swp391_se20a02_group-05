namespace CVerify.AI.Agents.SkillExtractionAgent;

public record ExtractedSkillsResult(
    string[] Skills,
    string[] Categories,
    string[] ProficiencyLevels);
