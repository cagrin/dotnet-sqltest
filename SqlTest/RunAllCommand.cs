namespace SqlTest;

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using SQLCover;

public class RunAllCommand : IDisposable
{
    private readonly RunAllOptions options;

    private readonly IConsole console;

    private readonly StopwatchLog stopwatchLogAll;

    private string database = "database_tests";

    private TestcontainerTarget target;

    private ITestcontainer? testcontainer;

    private CodeCoverage? coverage;

    private CoverageResult? code;

    public RunAllCommand(RunAllOptions options, IConsole? mockConsole = null)
    {
        this.options = options;
        this.console = mockConsole ?? SystemConsole.This;
        this.stopwatchLogAll = new StopwatchLog();
        this.target = new TestcontainerTarget();
    }

    public int Invoke()
    {
        _ = this.stopwatchLogAll.Start();

        this.PrepareDatabase();

        if (this.DeployDatabase())
        {
            this.RunTests();
            return this.ShowResults();
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

    private void PrepareDatabase()
    {
        var stopwatchLog = new StopwatchLog().Start("Preparing database...");

        var createContainerTask = this.CreateContainer();
        var buildDatabaseTask = this.CleanBuildDatabase();

        _ = Task.WhenAll(createContainerTask, buildDatabaseTask).Wait(millisecondsTimeout: 120000);

        stopwatchLog.Stop();
    }

    private async Task CreateContainer()
    {
        this.testcontainer = TestcontainerFactory.Create(this.options.Image);

        this.target = await this.testcontainer.StartAsync(this.options.Image, this.options.Collation).ConfigureAwait(false);
    }

    private async Task CleanBuildDatabase()
    {
        this.options.Project = DotnetTool.GetProjectFullName(this.options.Project);

        this.database = DotnetTool.GetDatabaseName(this.options.Project);

        string script = DotnetTool.GetCleanBuildScript(this.options.Project);

        _ = SystemConsole.Invoke(script);

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private bool DeployDatabase()
    {
        var stopwatchLog = new StopwatchLog().Start("Deploying database...");

        string script = DotnetTool.GetPublishScriptWithReferences(this.options.Project, this.target.TargetPort, this.database, this.target.TargetPassword);

        var results = SystemConsole.Invoke(script);

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

    private void RunTests()
    {
        var stopwatchLog = new StopwatchLog().Start("Running all tests....");

        this.coverage = new CodeCoverage(this.target.TargetConnectionString, this.database, this.options.CcIncludeTsqlt ? null : new[] { ".*tSQLt[.|\\]].*" });

        using var con = new SqlConnection(this.target.TargetConnectionString);

        try
        {
            if (!this.options.CcDisable)
            {
                _ = this.coverage.Start();
            }

            _ = con.Execute($"EXEC [{this.database}].tSQLt.RunAll", commandTimeout: 0);

            stopwatchLog.Stop();

            if (!this.options.CcDisable)
            {
                stopwatchLog = new StopwatchLog().Start("Gathering coverage...");

                this.code = this.coverage.Stop();

                this.CoberturaXml();

                stopwatchLog.Stop();
            }
        }
        catch (SqlException ex)
        {
            if (!this.options.CcDisable)
            {
                this.code = this.coverage.Stop();
            }

            stopwatchLog.Stop();

            this.console.ForegroundColor = ConsoleColor.Red;
            this.console.WriteLine(ex.Message);
            this.console.ResetColor();
        }
    }

    private int ShowResults()
    {
        int results = this.ResultLog();

        this.ResultXml();

        return results;
    }

    private int ResultLog()
    {
        using var con = new SqlConnection(this.target.TargetConnectionString);

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

    private void ResultXml()
    {
        if (!string.IsNullOrEmpty(this.options.Result))
        {
            string sql = $"[{this.database}].tSQLt.XmlResultFormatter";

            using var con = new SqlConnection(this.target.TargetConnectionString);
            using var file = new StreamWriter(this.options.Result);

            string xml = con.Query<string>(sql, CommandType.StoredProcedure).First();
            file.Write(xml);
        }
    }

    private void CoberturaXml()
    {
        if (!string.IsNullOrEmpty(this.options.CcCobertura))
        {
            using var file = new StreamWriter(this.options.CcCobertura);

            string xml = this.code!.Cobertura(this.database);
            file.Write(xml);
        }
    }
}