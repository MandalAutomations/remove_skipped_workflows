
using System;
using Octokit;

class GH
{
    private readonly GitHubClient github;
    private readonly string? token;
    private readonly Credentials? tokenAuth;

    public GH()
    {
        github = new GitHubClient(new ProductHeaderValue("octo-test-app"));
        token = Environment.GetEnvironmentVariable("GH_PAT");
        tokenAuth = new Credentials(token);
        github.Credentials = tokenAuth;
    }

    public async Task<string[]> GetRepos(string org)
    {
        var repos = await github.Repository.GetAllForOrg(org);
        Console.WriteLine($"Repositories for organization {repos.Count()}:");

        var repoNames = repos.Select(r => r.Name).ToArray();
        return repoNames;
    }
}