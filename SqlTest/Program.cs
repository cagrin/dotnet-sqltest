namespace SqlTest;

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

public static class Program
{
    public static int Result { get; set; }

    public static int Main(string[] args)
    {
        var runAll = new Command("runall", "Run all tests.")
        {
            new Option<string>(new[] { "--image", "-i" }, "Docker image."),
            new Option<string>(new[] { "--project", "-p" }, "Database project."),
            new Option<string>(new[] { "--collation", "-c" }, "Server collation."),
            new Option<string>(new[] { "--result", "-r" }, "Save result in JUnit XML file."),
            new Option<bool>(new[] { "--cc-disable" }, "Disable code coverage."),
            new Option<bool>(new[] { "--cc-include-tsqlt" }, "Include code coverage of tSQLt schema."),
        };

        runAll.Handler = CommandHandler.Create<RunAllOptions>(InvokeRunAll);

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            runAll,
        };

        return rootCommand.Invoke(args) + Result;
    }

    public static void InvokeRunAll(RunAllOptions options)
    {
        using var stc = new RunAllCommand(options);

        Result = stc.Invoke();
    }
}