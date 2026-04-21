namespace SqlTest;

using System.CommandLine;
using System.Globalization;

public static class Program
{
    public static int Main(string[] args)
    {
        var englishCulture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = englishCulture;
        CultureInfo.DefaultThreadCurrentUICulture = englishCulture;
        Environment.SetEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US");

        var projectOption = new Option<string>("--project", "-p") { Description = "Database project", Required = true };
        var imageOption = new Option<string>("--image", "-i") { Description = "Docker image" };
        var collationOption = new Option<string>("--collation", "-c") { Description = "Server collation" };
        var targetConnectionStringOption = new Option<string>("--connection-string", "-cs") { Description = "Target SQL connection string (mutually exclusive with --image)" };
        var resultOption = new Option<string>("--result", "-r") { Description = "Save result to JUnit XML file" };
        var ccCoberturaOption = new Option<string>("--cc-cobertura") { Description = "Save code coverage to Cobertura XML file" };
        var ccDisableOption = new Option<bool>("--cc-disable") { Description = "Disable code coverage" };
        var ccIncludeTsqltOption = new Option<bool>("--cc-include-tsqlt") { Description = "Include code coverage of tSQLt schema" };

        var runAll = new Command("runall", "Run all tests")
        {
            projectOption,
            imageOption,
            collationOption,
            targetConnectionStringOption,
            resultOption,
            ccCoberturaOption,
            ccDisableOption,
            ccIncludeTsqltOption,
        };

        runAll.SetAction(parseResult =>
        {
            var cs = parseResult.GetValue(targetConnectionStringOption);
            return InvokeRunAll(new RunAllOptions()
            {
                Project = parseResult.GetRequiredValue(projectOption),
                Image = parseResult.GetValue(imageOption),
                Collation = parseResult.GetValue(collationOption),
                UseExplicitTarget = !string.IsNullOrEmpty(cs),
                TargetConnectionString = cs,
                Result = parseResult.GetValue(resultOption),
                CcCobertura = parseResult.GetValue(ccCoberturaOption),
                CcDisable = parseResult.GetValue(ccDisableOption),
                CcIncludeTsqlt = parseResult.GetValue(ccIncludeTsqltOption),
            });
        });

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects")
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