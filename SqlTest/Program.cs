namespace SqlTest;

using System.CommandLine;

public static class Program
{
    public static int Result { get; set; }

    public static int Main(string[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args), "Value cannot be null.");
        }

        var imageOption = new Option<string>(new[] { "--image", "-i" }, "Docker image.");
        var projectOption = new Option<string>(new[] { "--project", "-p" }, "Database project.");
        var collationOption = new Option<string>(new[] { "--collation", "-c" }, "Server collation.") { IsRequired = false };
        var resultOption = new Option<string>(new[] { "--result", "-r" }, "Save result in JUnit XML file.") { IsRequired = false };
        var ccDisableOption = new Option<bool>(new[] { "--cc-disable" }, "Disable code coverage.") { IsRequired = false };
        var ccIncludeTsqltOption = new Option<bool>(new[] { "--cc-include-tsqlt" }, "Include code coverage of tSQLt schema.") { IsRequired = false };
        var windowsContainerOption = new Option<bool>(new[] { "--windows-container" }, "Run as Windows container.") { IsRequired = false };

        var runAll = new Command("runall", "Run all tests.")
        {
            imageOption,
            projectOption,
            collationOption,
            resultOption,
            ccDisableOption,
            ccIncludeTsqltOption,
            windowsContainerOption,
        };

        runAll.SetHandler(
            (string image, string project, string result, string collation, bool ccDisable, bool ccIncludeTsqlt, bool windowsContainer) =>
            InvokeRunAll(image, project, result, collation, ccDisable, ccIncludeTsqlt, windowsContainer),
            imageOption,
            projectOption,
            collationOption,
            resultOption,
            ccDisableOption,
            ccIncludeTsqltOption,
            windowsContainerOption);

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            runAll,
        };

        return rootCommand.Invoke(args) + Result;
    }

    public static void InvokeRunAll(string image, string project, string collation, string result, bool ccDisable, bool ccIncludeTsqlt, bool windowsContainer)
    {
        using var stc = new RunAllCommand();

        Result = stc.Invoke(image, project, collation, result, ccDisable, ccIncludeTsqlt, windowsContainer);
    }
}