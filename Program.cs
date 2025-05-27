using Octokit;

class Program
{
    static async Task Main(string[] args)
    {
        string orgName = "Xebia";
        var gh = new GH();

        var repos = await gh.GetRepos(orgName);

        foreach (var repo in repos)
        {
            Console.WriteLine($"{repo}");
        }
    }
}