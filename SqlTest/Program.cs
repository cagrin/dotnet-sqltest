namespace SqlTest;

using System.CommandLine;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args), "Value cannot be null.");
        }

        var imageOption = new Option<string>(new[] { "--image", "-i" }, "Docker image.");
        var projectOption = new Option<string>(new[] { "--project", "-p" }, "Database project.");

        var runAll = new Command("runall", "Run all tests.")
        {
            imageOption,
            projectOption,
        };

        runAll.SetHandler((string image, string project) => InvokeSqlTest(image, project), imageOption, projectOption);

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            runAll,
        };

        _ = rootCommand.Invoke(args);
    }

    public static void InvokeSqlTest(string image, string project)
    {
        using var stc = new SqlTestContainer();

        _ = stc.InvokeProject(image, project);
    }
}