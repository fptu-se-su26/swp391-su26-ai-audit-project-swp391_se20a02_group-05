namespace CVerify.AI.GitHub;

public interface IRepoSelector
{
    object[] Select(object[] repos, RepoSelectionCriteria criteria);
}

public record RepoSelectionCriteria(
    int MaxRepos,
    int MinCommits,
    bool PreferOwnedOver,
    string SortBy);
