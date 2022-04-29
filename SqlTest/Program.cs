namespace SqlTest;

using System.CommandLine;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args), "Value cannot be null.");
        }

        var imageOption = new Option<string>(new[] { "--image", "-i" }, "Docker image.");
        var projectOption = new Option<string>(new[] { "--project", "-p" }, "Database project.");
        var collationOption = new Option<string>(new[] { "--collation", "-c" }, "Server collation.") { IsRequired = false };

        var runAll = new Command("runall", "Run all tests.")
        {
            imageOption,
            projectOption,
            collationOption,
        };

        runAll.SetHandler((string image, string project, string collation) => InvokeRunAll(image, project, collation), imageOption, projectOption, collationOption);

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            runAll,
        };

        return rootCommand.Invoke(args);
    }

    public static void InvokeRunAll(string image, string project, string collation)
    {
        using var stc = new RunAllCommand();

        stc.Invoke(image, project, collation);
    }
}