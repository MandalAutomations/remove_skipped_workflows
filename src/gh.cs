
using System;
using Octokit;
using OctoTest.Mod;
class GH
{
    private readonly GitHubClient github;
    private readonly string? token;
    private readonly Credentials? tokenAuth;

    public GH()
    {
        github = new GitHubClient(new ProductHeaderValue("octo-test-app"));
        token = Environment.GetEnvironmentVariable("GH_TOKEN");
        tokenAuth = new Credentials(token);
        github.Credentials = tokenAuth;
    }
    public async Task<RepoModel[]> GetRepos(string org)
    {
        var repos = await github.Repository.GetAllForOrg(org);
        var mods = repos.Select(repo => new RepoModel
        {
            Id = repo.Id,
            Name = repo.Name,
            FullName = repo.FullName,
            Description = repo.Description,
            HtmlUrl = repo.HtmlUrl,
            Private = repo.Private,
            Language = repo.Language,
            StargazersCount = repo.StargazersCount,
            ForksCount = repo.ForksCount,
            Visibility = repo.Visibility?.ToString() ?? "Unknown"
        }).ToArray();
        return mods;
    }

    public async Task<RepoModel?> GetRepo(string org, string repoName)
    {
        try
        {
            var repo = await github.Repository.Get(org, repoName);
            return new RepoModel
            {
                Id = repo.Id,
                Name = repo.Name,
                FullName = repo.FullName,
                Description = repo.Description,
                HtmlUrl = repo.HtmlUrl,
                Private = repo.Private,
                Language = repo.Language,
                StargazersCount = repo.StargazersCount,
                ForksCount = repo.ForksCount,
                Visibility = repo.Visibility?.ToString() ?? "Unknown"
            };
        }
        catch (NotFoundException)
        {
            return null;
        }
    }
}