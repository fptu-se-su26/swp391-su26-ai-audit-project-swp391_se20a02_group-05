namespace CVerify.AI.GitHub;

public interface ITechnologyDetector
{
    string[] DetectFromPackageFiles(object[] files);
    string[] DetectFromFilenames(string[] filenames);
}
