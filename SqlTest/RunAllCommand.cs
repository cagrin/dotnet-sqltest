namespace SqlTest;

public sealed class RunAllCommand
{
    public const int ExitCodeSuccess = 0;

    public const int ExitCodeTestsFailed = 1;

    public const int ExitCodeError = 2;

    public const string TsqltExcludeFilter = @".*tSQLt[.|\]].*";

    private readonly RunAllOptions options;

    private readonly ITargetProvider targetProvider;

    private readonly DotnetTool dotnet;

    private readonly ISqlServerClient sqlClient;

    private readonly ICodeCoverage codeCoverage;

    private readonly IConsole console;

    private readonly TimeProvider timeProvider;

    public RunAllCommand(RunAllOptions options, ITargetProvider targetProvider, DotnetTool dotnet, ISqlServerClient sqlClient, ICodeCoverage codeCoverage, IConsole console, TimeProvider timeProvider)
    {
        this.options = options;
        this.targetProvider = targetProvider;
        this.dotnet = dotnet;
        this.sqlClient = sqlClient;
        this.codeCoverage = codeCoverage;
        this.console = console;
        this.timeProvider = timeProvider;
    }

    public async Task<int> InvokeAsync(CancellationToken cancellationToken = default)
    {
        var total = this.StartLog();

        try
        {
            var project = SqlProject.Load(this.options.Project);
            string database = this.GetDatabaseName(project);

            var target = await this.PrepareDatabaseAsync(project, cancellationToken).ConfigureAwait(false);

            if (!await this.DeployDatabaseAsync(project, target, database, cancellationToken).ConfigureAwait(false))
            {
                return ExitCodeError;
            }

            var coverage = await this.RunTestsAsync(target, database, cancellationToken).ConfigureAwait(false);

            return await this.ShowResultsAsync(target, database, coverage, total, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is SqlTestException or TimeoutException or ArgumentException)
        {
            this.WriteLines(ConsoleColor.Red, ex.Message);
            return ExitCodeError;
        }
    }

    private string GetDatabaseName(SqlProject project)
    {
        if (this.options.UseExplicitTarget)
        {
            string database = new RunTarget(this.options.TargetConnectionString).TargetDatabaseName;

            if (!string.IsNullOrWhiteSpace(database))
            {
                return database;
            }
        }

        return project.DatabaseName;
    }

    private async Task<RunTarget> PrepareDatabaseAsync(SqlProject project, CancellationToken cancellationToken)
    {
        var log = this.StartLog("Preparing database...");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(this.options.PrepareTimeout);

        var targetTask = this.targetProvider.StartAsync(timeout.Token);
        var buildTask = this.dotnet.CleanBuildAsync(project.FullName, timeout.Token);

        try
        {
            // Build errors are reported by publish, which builds the project again.
            await Task.WhenAll(targetTask, buildTask).ConfigureAwait(false);

            return await targetTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"Preparing database timed out after {this.options.PrepareTimeout}.");
        }
        finally
        {
            log.Stop();
        }
    }

    private async Task<bool> DeployDatabaseAsync(SqlProject project, RunTarget target, string database, CancellationToken cancellationToken)
    {
        var log = this.StartLog("Deploying database...");

        var result = await this.dotnet.PublishAsync(project, target, database, cancellationToken).ConfigureAwait(false);

        log.Stop();

        if (result.ExitCode == 0)
        {
            return true;
        }

        this.WriteLines(ConsoleColor.Red, result.StandardOutput.Concat(result.StandardError).Append("Deploying database failed.").ToArray());
        return false;
    }

    private async Task<CoverageReport?> RunTestsAsync(RunTarget target, string database, CancellationToken cancellationToken)
    {
        var log = this.StartLog("Running all tests....");

        if (!this.options.CcDisable)
        {
            this.codeCoverage.Start(target.TargetConnectionString, database, this.options.CcIncludeTsqlt ? null : [TsqltExcludeFilter]);
        }

        string? error = await this.sqlClient.RunAllTestsAsync(target.TargetConnectionString, database, cancellationToken).ConfigureAwait(false);

        log.Stop();

        if (error != null)
        {
            this.WriteLines(ConsoleColor.Red, error);
        }

        if (this.options.CcDisable)
        {
            return null;
        }

        log = this.StartLog("Gathering coverage...");

        var coverage = this.codeCoverage.Stop();

        if (!string.IsNullOrEmpty(this.options.CcCobertura))
        {
            await File.WriteAllTextAsync(this.options.CcCobertura, coverage.CoberturaXml, cancellationToken).ConfigureAwait(false);
        }

        log.Stop();

        return coverage;
    }

    private async Task<int> ShowResultsAsync(RunTarget target, string database, CoverageReport? coverage, StopwatchLog total, CancellationToken cancellationToken)
    {
        var results = await this.sqlClient.GetTestResultsAsync(target.TargetConnectionString, database, cancellationToken).ConfigureAwait(false);

        int passed = results.Count(p => p.Result == "Success");
        var failures = results.Where(p => p.Result is "Failure" or "Error").ToArray();

        if (failures.Length > 0)
        {
            this.WriteLines(ConsoleColor.Red, ["Failed test messages:", .. failures.Select(p => $"  {p.Name}: {p.Msg}")]);
        }

        string cc = string.Empty;

        if (coverage != null)
        {
            if (coverage.Uncovered.Count > 0)
            {
                this.WriteLines(ConsoleColor.Yellow, ["Uncovered statements:", .. coverage.Uncovered.Select(p => $"  {p.ObjectName}: {p.Text.FirstLine()}")]);
            }

            cc = $", Coverage: {coverage.Percent}% ({coverage.CoveredStatementCount}/{coverage.StatementCount})";
        }

        this.console.ForegroundColor = failures.Length > 0 ? ConsoleColor.Red : ConsoleColor.Green;
        this.console.Write($"Failed: {failures.Length}, Passed: {passed}{cc}, Duration:");
        total.Stop();
        this.console.ResetColor();

        if (!string.IsNullOrEmpty(this.options.Result))
        {
            string xml = await this.sqlClient.GetJUnitXmlAsync(target.TargetConnectionString, database, cancellationToken).ConfigureAwait(false);

            await File.WriteAllTextAsync(this.options.Result, xml, cancellationToken).ConfigureAwait(false);
        }

        return failures.Length > 0 ? ExitCodeTestsFailed : ExitCodeSuccess;
    }

    private StopwatchLog StartLog(string title = "")
    {
        return new StopwatchLog(this.console, this.timeProvider).Start(title);
    }

    private void WriteLines(ConsoleColor color, params string[] lines)
    {
        this.console.ForegroundColor = color;

        foreach (string line in lines)
        {
            this.console.WriteLine(line);
        }

        this.console.ResetColor();
    }
}