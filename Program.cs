using Octokit;

class Program
{
    static async Task Main(string[] args)
    {

        string orgName = null;
        string repoName = null;

        if (args.Length != 0)
        {
            var command = args[0].ToLowerInvariant();
            Console.WriteLine($"Command: {command}");
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

        if (string.IsNullOrEmpty(orgName))
        {
            Console.WriteLine("Organization name is required. Use --org org_name");
            return;
        }

        Console.WriteLine($"Organization: {orgName}");
        if (!string.IsNullOrEmpty(repoName))
            Console.WriteLine($"Repository: {repoName}");

        // Example usage:
        // var gh = new GH();
        // var repos = await gh.GetRepos(orgName);
        // foreach (var repo in repos)
        // {
        //     Console.WriteLine($"{repo}");
        // }
    }
}