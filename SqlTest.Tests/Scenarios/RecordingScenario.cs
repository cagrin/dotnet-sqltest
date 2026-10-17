namespace SqlTest.Tests;

using System.Globalization;

// Passes calls to real adapters and records their results, so a run against real infrastructure can refresh fixtures.
internal sealed class RecordingScenario : IProcessRunner, ISqlServerClient, ICodeCoverage
{
    public const string RecordVariable = "SQLTEST_RECORD_FIXTURES";

    private readonly IProcessRunner processRunner;

    private readonly ISqlServerClient sqlClient;

    private readonly ICodeCoverage codeCoverage;

    public RecordingScenario(IProcessRunner processRunner, ISqlServerClient sqlClient, ICodeCoverage codeCoverage)
    {
        this.processRunner = processRunner;
        this.sqlClient = sqlClient;
        this.codeCoverage = codeCoverage;
    }

    public RecordingScenario()
        : this(new ProcessRunner(), new SqlServerClient(), new SqlCoverCodeCoverage())
    {
    }

    public Scenario Scenario { get; } = new Scenario();

    // Program.Main forces en-US; the culture is async-local, so it only applies to the calling test.
    public static void UseToolCulture()
    {
        var englishCulture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.CurrentCulture = englishCulture;
        CultureInfo.CurrentUICulture = englishCulture;
    }

    public async Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        var result = await this.processRunner.RunAsync(fileName, arguments, cancellationToken).ConfigureAwait(false);

        lock (this.Scenario)
        {
            this.Scenario.Processes.Add(RecordedProcess.From(arguments, result));
        }

        return result;
    }

    public Task PingAsync(string connectionString, CancellationToken cancellationToken)
    {
        return this.sqlClient.PingAsync(connectionString, cancellationToken);
    }

    public async Task<string?> RunAllTestsAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        this.Scenario.RunAllError = await this.sqlClient.RunAllTestsAsync(connectionString, database, cancellationToken).ConfigureAwait(false);
        return this.Scenario.RunAllError;
    }

    public async Task<IReadOnlyList<TestResult>> GetTestResultsAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        var results = await this.sqlClient.GetTestResultsAsync(connectionString, database, cancellationToken).ConfigureAwait(false);
        this.Scenario.TestResults.AddRange(results);
        return results;
    }

    public async Task<string> GetJUnitXmlAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        this.Scenario.JUnitXml = await this.sqlClient.GetJUnitXmlAsync(connectionString, database, cancellationToken).ConfigureAwait(false);
        return this.Scenario.JUnitXml;
    }

    public void Start(string connectionString, string database, string[]? excludeFilter)
    {
        this.codeCoverage.Start(connectionString, database, excludeFilter);
    }

    public CoverageReport Stop()
    {
        this.Scenario.Coverage = this.codeCoverage.Stop();
        return this.Scenario.Coverage;
    }

    public void SaveIfRequested(string name)
    {
        if (Environment.GetEnvironmentVariable(RecordVariable) == "1")
        {
            this.Scenario.Save(TestPaths.FixturesSource, name);
        }
    }
}
