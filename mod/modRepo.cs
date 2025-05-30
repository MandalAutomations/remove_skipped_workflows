using Octokit;

namespace OctoTest.Mod
{
    public class RepoModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string HtmlUrl { get; set; } = string.Empty;
        public bool Private { get; set; }
        public string? Language { get; set; }
        public int StargazersCount { get; set; }
        public int ForksCount { get; set; }

        public RepoModel() { }

        public string? Visibility { get; set; }

        public RepoModel(Repository repo)
        {
            Id = repo.Id;
            Name = repo.Name;
            FullName = repo.FullName;
            Description = repo.Description;
            HtmlUrl = repo.HtmlUrl;
            Private = repo.Private;
            Language = repo.Language;
            StargazersCount = repo.StargazersCount;
            ForksCount = repo.ForksCount;
            Visibility = repo.Visibility.ToString();
        }
    }
}