using Octokit;

class Program
{
    static async Task Main(string[] args)
    {
        string? orgName = null;
        string? repoName = null;
        string? command = null;

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GH_TOKEN")))
        {
            Console.WriteLine("need token");
            return;
        }

        if (args.Length != 0)
        {
            command = args[0].ToLowerInvariant();
        }
        else
        {
            Console.WriteLine("No command provided.");
            return;
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--org" && i + 1 < args.Length)
            {
                orgName = args[i + 1];
                i++;
            }
            else if (args[i] == "--repo" && i + 1 < args.Length)
            {
                repoName = args[i + 1];
                i++;
            }
        }

        var gh = new GH();

        if (command == "orgrepos" && !string.IsNullOrEmpty(orgName))
        {
            var repos = await gh.GetRepos(orgName);
            foreach (var repo in repos)
            {
                Console.WriteLine($"{repo.Name} - {repo.Visibility} - {repo.Language}");
            }
        }
        else if (command == "repo" && !string.IsNullOrEmpty(repoName) && !string.IsNullOrEmpty(orgName))
        {
            var repo = await gh.GetRepo(orgName, repoName);
            if (repo != null)
            {
                Console.WriteLine($"Repo: {repo.Name}");
                Console.WriteLine($"Description: {repo.Description}");
                Console.WriteLine($"URL: {repo.HtmlUrl}");
                Console.WriteLine($"Visibility: {repo.Visibility}");
                Console.WriteLine($"Language: {repo.Language}");
                Console.WriteLine($"Stars: {repo.StargazersCount}");
                Console.WriteLine($"Forks: {repo.ForksCount}");
            }
            else
            {
                Console.WriteLine("Repository not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid command or missing parameters.");
            Console.WriteLine("Usage:");
            Console.WriteLine("  orgrepos --org <organization_name>");
            Console.WriteLine("  repo --org <organization_name> --repo <repository_name>");
        }
    }
}