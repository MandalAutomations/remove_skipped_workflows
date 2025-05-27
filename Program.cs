using Octokit;

class Program
{
    static async Task Main(string[] args)
    {
        var github = new GitHubClient(new ProductHeaderValue("octo-test-app"));
        var token = Environment.GetEnvironmentVariable("GH_PAT");
        var tokenAuth = new Credentials(token);
        github.Credentials = tokenAuth;

        string orgName = "Xebia";
        var gh = new GH();

        var repos = await gh.GetRepos(orgName);

        foreach (var repo in repos)
        {
            Console.WriteLine($"{repo}");
        }
    }
}