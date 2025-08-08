namespace SqlTest;

using System.CommandLine;

public static class Program
{
    public static int Main(string[] args)
    {
        var imageOption = new Option<string>("--image", "-i") { Description = "Docker image." };
        var projectOption = new Option<string>("--project", "-p") { Description = "Database project." };
        var collationOption = new Option<string>("--collation", "-c") { Description = "Server collation." };
        var resultOption = new Option<string>("--result", "-r") { Description = "Save result to JUnit XML file." };
        var ccCoberturaOption = new Option<string>("--cc-cobertura") { Description = "Save code coverage to Cobertura XML file." };
        var ccDisableOption = new Option<bool>("--cc-disable") { Description = "Disable code coverage." };
        var ccIncludeTsqltOption = new Option<bool>("--cc-include-tsqlt") { Description = "Include code coverage of tSQLt schema." };

        var runAll = new Command("runall", "Run all tests.")
        {
            imageOption,
            projectOption,
            collationOption,
            resultOption,
            ccCoberturaOption,
            ccDisableOption,
            ccIncludeTsqltOption,
        };

        runAll.SetAction(parseResult => InvokeRunAll(new RunAllOptions()
        {
            Image = parseResult.GetValue(imageOption) ?? string.Empty,
            Project = parseResult.GetValue(projectOption) ?? string.Empty,
            Collation = parseResult.GetValue(collationOption) ?? string.Empty,
            Result = parseResult.GetValue(resultOption) ?? string.Empty,
            CcCobertura = parseResult.GetValue(ccCoberturaOption) ?? string.Empty,
            CcDisable = parseResult.GetValue(ccDisableOption),
            CcIncludeTsqlt = parseResult.GetValue(ccIncludeTsqltOption),
        }));

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            runAll,
        };

        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }

    public static int InvokeRunAll(RunAllOptions options)
    {
        using var stc = new RunAllCommand(options);

        return stc.Invoke();
    }
}