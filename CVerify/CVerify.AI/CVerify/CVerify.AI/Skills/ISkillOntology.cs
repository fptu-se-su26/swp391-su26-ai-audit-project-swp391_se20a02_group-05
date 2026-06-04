namespace CVerify.AI.Skills;

public interface ISkillOntology
{
    Task<SkillDefinition> GetSkillAsync(string skillName);
    Task<string[]> GetAllSkillsAsync();
    Task<string[]> GetSkillsByCategory(string category);
}

public record SkillDefinition(
    string Name,
    string Category,
    string[] Aliases,
    int Proficiency);
