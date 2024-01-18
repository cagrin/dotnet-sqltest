namespace SqlTest;

using System.Data;
using System.Data.SqlClient;
using Dapper;
using SQLCover;

public class RunAllCommand : IDisposable
{
    private readonly IConsole console;

    private readonly StopwatchLog stopwatchLogAll = new StopwatchLog();

    private readonly string database = "database_tests";

    private string password = string.Empty;

    private int port;

    private string cs = string.Empty;

    private ITestcontainer? testcontainer;

    private CodeCoverage? coverage;

    private CoverageResult? code;

    public RunAllCommand(IConsole? mockConsole = null)
    {
        this.console = mockConsole ?? SystemConsole.This;
    }

    public int Invoke(string image, string project, string collation, string result, bool ccDisable, bool ccIncludeTsqlt)
    {
        _ = this.stopwatchLogAll.Start();

        this.PrepareDatabase(image, project, collation);

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
        if (disposing)
        {
            this.testcontainer?.Dispose();
        }
    }

    private void PrepareDatabase(string image, string project, string collation)
    {
        var stopwatchLog = new StopwatchLog().Start("Preparing database...");

        var createContainerTask = this.CreateContainer(image, collation);
        var buildDatabaseTask = this.CleanBuildDatabase(project);

        _ = Task.WhenAll(createContainerTask, buildDatabaseTask).Wait(millisecondsTimeout: 120000);

        stopwatchLog.Stop();
    }

    private async Task CreateContainer(string image, string collation)
    {
        this.testcontainer = TestcontainerFactory.Create(image);

        var (password, port, cs) = await this.testcontainer.StartAsync(image, collation).ConfigureAwait(false);

        this.password = password;
        this.port = port;
        this.cs = cs;
    }

    private async Task CleanBuildDatabase(string project)
    {
        _ = this.database;

        string script = $"dotnet clean {project}\ndotnet build {project}";

        _ = await PowerShellConsole.InvokeAsync(script).ConfigureAwait(false);
    }

    private bool DeployDatabase(string project)
    {
        var stopwatchLog = new StopwatchLog().Start("Deploying database...");

        string script = $"dotnet publish {project} /p:TargetServerName=localhost /p:TargetPort={this.port} /p:TargetDatabaseName={this.database} /p:TargetUser=sa /p:TargetPassword=\"{this.password}\" --nologo";

        var results = PowerShellConsole.Invoke(script);

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
            if (uncoveredBatches.Length > 0)
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