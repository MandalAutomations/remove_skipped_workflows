using Octokit;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        GitHubClient github = new GitHubClient(new ProductHeaderValue("octo-test-app"));
        string? token = Environment.GetEnvironmentVariable("GH_TOKEN");
        Credentials? tokenAuth = new Credentials(token);
        github.Credentials = tokenAuth;

        string repoFullName = args.Length > 0 ? args[0] : throw new ArgumentException("Repository (org/repo) must be provided as the first argument.");
        var repoParts = repoFullName.Split('/');
        if (repoParts.Length != 2)
            throw new ArgumentException("Repository must be in the format 'org/repo'.");

        string owner = repoParts[0];
        string repo = repoParts[1];

        int total = 0;
        int totalDeleted = 0;
        int perPage = 100;
        int page = 1;

        while (true)
        {
            var runs = await github.Actions.Workflows.Runs.List(owner, repo, new WorkflowRunsRequest() { });

            var skippedRuns = runs.WorkflowRuns?.Where(r => r.Conclusion == "skipped").ToList();
            if (skippedRuns == null || skippedRuns.Count == 0)
                break;

            total = skippedRuns.Count;
            foreach (var run in skippedRuns)
            {
                await github.Connection.Delete(new Uri($"/repos/{owner}/{repo}/actions/runs/{run.Id}", UriKind.Relative), null, "application/vnd.github.v3+json");
                totalDeleted++;
                if (totalDeleted % 10 == 0)
                {
                    Console.WriteLine($"Total deleted so far: {totalDeleted}/{total}");
                }
            }

            if (skippedRuns.Count < perPage)
                break;

            page++;
        }

        Console.WriteLine($"Total deleted: {totalDeleted}");
    }
}