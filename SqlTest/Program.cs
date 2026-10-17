namespace SqlTest;

using System.CommandLine;
using System.Globalization;

public static class Program
{
    public static Task<int> Main(string[] args)
    {
        var englishCulture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = englishCulture;
        CultureInfo.DefaultThreadCurrentUICulture = englishCulture;
        Environment.SetEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US");

        return RunAsync(args, InvokeRunAllAsync);
    }

    public static Task<int> RunAsync(string[] args, Func<RunAllOptions, CancellationToken, Task<int>> runAll, InvocationConfiguration? configuration = null)
    {
        var projectOption = new Option<string>("--project", "-p") { Description = "Database project", Required = true };
        var imageOption = new Option<string>("--image", "-i") { Description = "Docker image" };
        var collationOption = new Option<string>("--collation", "-c") { Description = "Server collation" };
        var targetOption = new Option<string>("--target", "-t") { Description = "Target connection string (mutually exclusive with --image)" };
        var resultOption = new Option<string>("--result", "-r") { Description = "Save result to JUnit XML file" };
        var ccCoberturaOption = new Option<string>("--cc-cobertura") { Description = "Save code coverage to Cobertura XML file" };
        var ccDisableOption = new Option<bool>("--cc-disable") { Description = "Disable code coverage" };
        var ccIncludeTsqltOption = new Option<bool>("--cc-include-tsqlt") { Description = "Include code coverage of tSQLt schema" };

        var runAllCommand = new Command("runall", "Run all tests")
        {
            projectOption,
            imageOption,
            collationOption,
            targetOption,
            resultOption,
            ccCoberturaOption,
            ccDisableOption,
            ccIncludeTsqltOption,
        };

        runAllCommand.Validators.Add(result =>
        {
            if (!string.IsNullOrEmpty(result.GetValue(imageOption)) && !string.IsNullOrEmpty(result.GetValue(targetOption)))
            {
                result.AddError("Options --image and --target are mutually exclusive.");
            }
        });

        runAllCommand.SetAction((parseResult, cancellationToken) => runAll(
            new RunAllOptions()
            {
                Project = parseResult.GetRequiredValue(projectOption),
                Image = parseResult.GetValue(imageOption),
                Collation = parseResult.GetValue(collationOption),
                TargetConnectionString = parseResult.GetValue(targetOption),
                Result = parseResult.GetValue(resultOption),
                CcCobertura = parseResult.GetValue(ccCoberturaOption),
                CcDisable = parseResult.GetValue(ccDisableOption),
                CcIncludeTsqlt = parseResult.GetValue(ccIncludeTsqltOption),
            },
            cancellationToken));

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects")
        {
            runAllCommand,
        };

        return rootCommand.Parse(args).InvokeAsync(configuration);
    }

    public static ITargetProvider CreateTargetProvider(RunAllOptions options, ISqlServerClient sqlClient)
    {
        return options.UseExplicitTarget
            ? new ExplicitTarget(options.TargetConnectionString, sqlClient)
            : new MsSqlTestcontainer(options.Image, options.Collation);
    }

    public static async Task<int> InvokeRunAllAsync(RunAllOptions options, CancellationToken cancellationToken)
    {
        var console = new SystemConsole();
        var sqlClient = new SqlServerClient();
#if DEBUG
        var processRunner = new ProcessRunner(echo: console);
#else
        var processRunner = new ProcessRunner();
#endif

        var targetProvider = CreateTargetProvider(options, sqlClient);

        await using (targetProvider.ConfigureAwait(false))
        {
            var command = new RunAllCommand(options, targetProvider, new DotnetTool(processRunner), sqlClient, new SqlCoverCodeCoverage(), console, TimeProvider.System);

            return await command.InvokeAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}