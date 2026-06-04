namespace CVerify.AI.Skills;

public class SkillNormalizer
{
    private readonly ISkillOntology _ontology;

    public SkillNormalizer(ISkillOntology ontology)
    {
        _ontology = ontology;
    }

    public async Task<string> NormalizeAsync(string skillName)
    {
        // Implementation will go here - map skill to ontology
        return skillName;
    }

    public async Task<string[]> NormalizeMultipleAsync(string[] skills)
    {
        // Implementation will go here
        return skills;
    }
}
