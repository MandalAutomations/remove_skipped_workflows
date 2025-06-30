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

        try
        {

            var repository = await github.Repository.Get(owner, repo);
            Console.WriteLine($"Found repository: {repository.FullName}");
        }
        catch (NotFoundException)
        {
            Console.WriteLine($"Repository '{owner}/{repo}' not found or you don't have access to it.");
            Console.WriteLine("Please check:");
            Console.WriteLine("1. The repository name is correct (case-sensitive)");
            Console.WriteLine("2. You have access to the repository");
            Console.WriteLine("3. Your GitHub token has the necessary permissions");
            return;
        }

        int totalDeleted = 0;
        int totalSkipped = 0;
        int page = 1;
        const int perPage = 100;

        Console.WriteLine("Searching for skipped workflow runs...");

        while (true)
        {
            try
            {
                var request = new WorkflowRunsRequest() 
                { 
                    Status = CheckRunStatusFilter.Completed
                };
                
                var options = new ApiOptions()
                {
                    PageSize = perPage,
                    PageCount = 1,
                    StartPage = page
                };
                
                var runs = await github.Actions.Workflows.Runs.List(owner, repo, request, options);

                var skippedRuns = runs.WorkflowRuns?.Where(r => r.Conclusion == "skipped").ToList();
                if (skippedRuns == null || skippedRuns.Count == 0)
                {
                    Console.WriteLine($"No more skipped runs found on page {page}");
                    break;
                }

                totalSkipped += skippedRuns.Count;
                Console.WriteLine($"Found {skippedRuns.Count} skipped runs on page {page}");

                foreach (var run in skippedRuns)
                {
                    try
                    {
                        Console.WriteLine($"Deleting run {run.Id} for workflow '{run.Name}' (Status: {run.Status}, Conclusion: {run.Conclusion})");
                        await github.Actions.Workflows.Runs.Delete(owner, repo, run.Id);
                        totalDeleted++;
                        if (totalDeleted % 10 == 0)
                        {
                            Console.WriteLine($"Deleted {totalDeleted} runs so far...");
                        }
                    }
                    catch (NotFoundException)
                    {
                        Console.WriteLine($"Workflow run {run.Id} not found (may have been already deleted)");
                    }
                    catch (ApiException ex)
                    {
                        Console.WriteLine($"Failed to delete workflow run {run.Id}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error deleting workflow run {run.Id}: {ex.Message}");
                    }
                }

                // If we got fewer results than requested, we're at the end
                if (runs.WorkflowRuns == null || runs.WorkflowRuns.Count < perPage)
                    break;

                page++;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error retrieving workflow runs: {ex.Message}");
                break;
            }
        }

        Console.WriteLine($"Total deleted: {totalDeleted}");
    }
}