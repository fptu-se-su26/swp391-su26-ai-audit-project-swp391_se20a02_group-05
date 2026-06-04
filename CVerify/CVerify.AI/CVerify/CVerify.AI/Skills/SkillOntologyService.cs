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

public class SkillOntologyService : ISkillOntology
{
    public async Task<SkillDefinition> GetSkillAsync(string skillName)
    {
        // Implementation will go here
        return new(skillName, "", Array.Empty<string>(), 0);
    }

    public async Task<string[]> GetAllSkillsAsync()
    {
        // Implementation will go here
        return Array.Empty<string>();
    }

    public async Task<string[]> GetSkillsByCategory(string category)
    {
        // Implementation will go here
        return Array.Empty<string>();
    }
}
