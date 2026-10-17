namespace SqlTest.Tests;

// Replays a recorded scenario in memory and remembers how it was called.
internal sealed class ReplayScenario : IProcessRunner, ISqlServerClient, ICodeCoverage
{
    private readonly Scenario scenario;

    private int nextProcess;

    public ReplayScenario(Scenario scenario)
    {
        this.scenario = scenario;
    }

    public List<IReadOnlyList<string>> ProcessCalls { get; } = [];

    public List<string> SqlCalls { get; } = [];

    public string[]? CoverageExcludeFilter { get; private set; }

    public bool CoverageStarted { get; private set; }

    public static ReplayScenario Load(string name)
    {
        return new ReplayScenario(Scenario.Load(name));
    }

    public Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        Assert.AreEqual(DotnetTool.FileName, fileName);

        var expected = this.scenario.Processes[this.nextProcess++];

        Assert.AreEqual(expected.Command, arguments[0]);
        Assert.AreEqual(expected.Project, Path.GetFileName(arguments[1]));

        this.ProcessCalls.Add(arguments);

        return Task.FromResult(expected.Result);
    }

    public Task PingAsync(string connectionString, CancellationToken cancellationToken)
    {
        this.SqlCalls.Add($"Ping {connectionString}");
        return Task.CompletedTask;
    }

    public Task<string?> RunAllTestsAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        this.SqlCalls.Add($"RunAll {database}");
        return Task.FromResult(this.scenario.RunAllError);
    }

    public Task<IReadOnlyList<TestResult>> GetTestResultsAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        this.SqlCalls.Add($"TestResults {database}");
        return Task.FromResult<IReadOnlyList<TestResult>>(this.scenario.TestResults);
    }

    public Task<string> GetJUnitXmlAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        this.SqlCalls.Add($"JUnitXml {database}");
        return Task.FromResult(this.scenario.JUnitXml);
    }

    public void Start(string connectionString, string database, string[]? excludeFilter)
    {
        this.CoverageStarted = true;
        this.CoverageExcludeFilter = excludeFilter;
    }

    public CoverageReport Stop()
    {
        Assert.IsTrue(this.CoverageStarted);
        return this.scenario.Coverage!;
    }
}
