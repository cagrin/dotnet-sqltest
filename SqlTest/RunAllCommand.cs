namespace SqlTest;

using System.Data;
using System.Data.SqlClient;
using Dapper;
using LikeComparison.TransactSql;
using SQLCover;
using Testcontainers.MsSql;

public class RunAllCommand : IDisposable
{
    private readonly IConsole console;

    private readonly StopwatchLog stopwatchLogAll = new StopwatchLog();

    private readonly string database = "database_tests";

    private string password = string.Empty;

    private int port;

    private string cs = string.Empty;

    private MsSqlContainer? testcontainer;

    private CodeCoverage? coverage;

    private CoverageResult? code;

    public RunAllCommand(IConsole? mockConsole = null)
    {
        this.console = mockConsole ?? SystemConsole.This;
    }

    public int Invoke(string image, string project, string collation, string result, bool ccDisable, bool ccIncludeTsqlt, bool windowsContainer)
    {
        _ = this.stopwatchLogAll.Start();

        this.PrepareDatabase(image, project, collation, windowsContainer);

        if (this.DeployDatabase(project))
        {
            this.RunTests(ccDisable, ccIncludeTsqlt);
            return this.ShowResults(result);
        }

        return 2;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }

    private void PrepareDatabase(string image, string project, string collation, bool windowsContainer)
    {
        var stopwatchLog = new StopwatchLog().Start("Preparing database...");

        var createContainerTask = this.CreateContainer(image, collation, windowsContainer);
        var buildDatabaseTask = this.CleanBuildDatabase(project);

        Task.WhenAll(createContainerTask, buildDatabaseTask).Wait();

        stopwatchLog.Stop();
    }

    private async Task CreateContainer(string image, string collation, bool windowsContainer)
    {
        this.testcontainer = MsSqlFactory.CreateTestcontainer(image, collation, windowsContainer);
        await this.testcontainer.StartAsync().ConfigureAwait(false);

        this.password = MsSqlBuilder.DefaultPassword;
        this.port = this.testcontainer.GetMappedPublicPort(MsSqlBuilder.MsSqlPort);
        this.cs = this.testcontainer.GetConnectionString();
    }

    private async Task CleanBuildDatabase(string project)
    {
        _ = this.database;

        string script = $"dotnet clean {project}\ndotnet build {project}";

        _ = await PowerShellCommand.InvokeAsync(script).ConfigureAwait(false);
    }

    private bool DeployDatabase(string project)
    {
        var stopwatchLog = new StopwatchLog().Start("Deploying database...");

        string script = $"dotnet publish {project} /p:TargetServerName=localhost /p:TargetPort={this.port} /p:TargetDatabaseName={this.database} /p:TargetUser=sa /p:TargetPassword=\"{this.password}\" --nologo";

        var results = PowerShellCommand.Invoke(script);

        stopwatchLog.Stop();

        if (!results.Last().ToString().Like("%Successfully deployed database%"))
        {
            string error = string.Empty;

            foreach (var result in results)
            {
                error += $"{result}\n";
            }

            error += "Deploying database failed.";

            this.console.ForegroundColor = ConsoleColor.Red;
            this.console.WriteLine(error);
            this.console.ResetColor();
            return false;
        }

        return true;
    }

    private void RunTests(bool ccDisable, bool ccIncludeTsqlt)
    {
        var stopwatchLog = new StopwatchLog().Start("Running all tests....");

        this.coverage = new CodeCoverage(this.cs, this.database, ccIncludeTsqlt ? null : new[] { ".*tSQLt[.|\\]].*" });

        using var con = new SqlConnection(this.cs);

        try
        {
            if (!ccDisable)
            {
                _ = this.coverage.Start();
            }

            _ = con.Execute($"EXEC [{this.database}].tSQLt.RunAll", commandTimeout: 0);

            stopwatchLog.Stop();

            if (!ccDisable)
            {
                stopwatchLog = new StopwatchLog().Start("Gathering coverage...");

                this.code = this.coverage.Stop();

                stopwatchLog.Stop();
            }
        }
        catch (SqlException ex)
        {
            if (!ccDisable)
            {
                this.code = this.coverage.Stop();
            }

            stopwatchLog.Stop();

            this.console.ForegroundColor = ConsoleColor.Red;
            this.console.WriteLine(ex.Message);
            this.console.ResetColor();
        }
    }

    private int ShowResults(string result)
    {
        int results = this.ResultLog();

        if (!string.IsNullOrEmpty(result))
        {
            this.ResultXml(result);
        }

        return results;
    }

    private int ResultLog()
    {
        using var con = new SqlConnection(this.cs);

        var results = con.Query<TestResult>($"SELECT Name, Result, Msg FROM [{this.database}].tSQLt.TestResult").ToArray();

        int passed = results.Where(p => p.Result == "Success").Count();
        int failed = results.Where(p => p.Result == "Failure").Count();

        if (failed > 0)
        {
            this.console.ForegroundColor = ConsoleColor.Red;
            this.console.WriteLine("Failed test messages:");
            foreach (var result in results.Where(p => p.Result == "Failure"))
            {
                this.console.WriteLine($"  {result.Name}: {result.Msg}");
            }
        }

        string cc = string.Empty;
        if (this.code != null)
        {
            var uncoveredBatches = this.code.Batches.Where(p => p.Statements.Any(r => r.HitCount == 0)).ToArray();
            if (uncoveredBatches.Any())
            {
                this.console.ForegroundColor = ConsoleColor.Yellow;
                this.console.WriteLine("Uncovered statements:");
                foreach (var batch in uncoveredBatches)
                {
                    foreach (var statement in batch.Statements.Where(p => p.HitCount == 0))
                    {
                        this.console.WriteLine($"  {batch.ObjectName}: {statement.Text.FirstLine()}");
                    }
                }
            }

            long cr = this.code.StatementCount == 0 ? 0 : Convert.ToInt64(Convert.ToDouble(this.code.CoveredStatementCount) / Convert.ToDouble(this.code.StatementCount) * 100.0);
            cc = $", Coverage: {cr}% ({this.code.CoveredStatementCount}/{this.code.StatementCount})";
        }

        this.console.ForegroundColor = failed > 0 ? ConsoleColor.Red : ConsoleColor.Green;
        this.console.Write($"Failed: {failed}, Passed: {passed}{cc}, Duration:");
        this.stopwatchLogAll.Stop();
        this.console.ResetColor();

        return (failed > 0) ? 1 : 0;
    }

    private void ResultXml(string result)
    {
        string sql = $"[{this.database}].tSQLt.XmlResultFormatter";

        using var con = new SqlConnection(this.cs);
        using var file = new StreamWriter(result);

        string xml = con.Query<string>(sql, CommandType.StoredProcedure).First();
        file.Write(xml);
    }
}