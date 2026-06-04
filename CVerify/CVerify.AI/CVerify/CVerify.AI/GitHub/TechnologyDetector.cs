namespace CVerify.AI.GitHub;

public interface ITechnologyDetector
{
    string[] DetectFromPackageFiles(object[] files);
    string[] DetectFromFilenames(string[] filenames);
}

public class TechnologyDetector : ITechnologyDetector
{
    public string[] DetectFromPackageFiles(object[] files)
    {
        // Implementation will go here - detect from package.json, *.csproj, etc.
        return Array.Empty<string>();
    }

    public string[] DetectFromFilenames(string[] filenames)
    {
        // Implementation will go here
        return Array.Empty<string>();
    }
}
