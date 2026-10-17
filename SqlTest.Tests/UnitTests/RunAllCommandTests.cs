namespace SqlTest.UnitTests;

[TestClass]
public class RunAllCommandTests
{
    private readonly FakeConsole console = new FakeConsole();

    private readonly ManualTimeProvider time = new ManualTimeProvider();

    public static IEnumerable<object[]> Scenarios => ScenarioCase.Names;

    [TestMethod]
    [DynamicData(nameof(Scenarios))]
    public async Task ShouldReplayScenario(string name)
    {
        var scenario = ScenarioCase.Get(name);
        var replay = ReplayScenario.Load(name);

        int exitCode = await this.CreateSut(scenario.ToOptions(), replay).InvokeAsync().ConfigureAwait(false);

        scenario.Verify(exitCode, this.console.Lines);
    }

    [TestMethod]
    public async Task ShouldExcludeTsqltFromCoverageByDefault()
    {
        var replay = ReplayScenario.Load("Ok");

        _ = await this.CreateSut(ScenarioCase.Get("Ok").ToOptions(), replay).InvokeAsync().ConfigureAwait(false);

        Assert.AreSequenceEqual(new[] { RunAllCommand.TsqltExcludeFilter }, replay.CoverageExcludeFilter!);
        Assert.AreSequenceEqual(new[] { "RunAll Database.Tests", "TestResults Database.Tests" }, replay.SqlCalls);
    }

    [TestMethod]
    public async Task ShouldIncludeTsqltInCoverage()
    {
        var replay = ReplayScenario.Load("OkIncludeTsqlt");

        _ = await this.CreateSut(ScenarioCase.Get("OkIncludeTsqlt").ToOptions(), replay).InvokeAsync().ConfigureAwait(false);

        Assert.IsTrue(replay.CoverageStarted);
        Assert.IsNull(replay.CoverageExcludeFilter);
    }

    [TestMethod]
    public async Task ShouldNotStartCoverageWhenDisabled()
    {
        var replay = ReplayScenario.Load("OkCcDisable");

        _ = await this.CreateSut(ScenarioCase.Get("OkCcDisable").ToOptions(), replay).InvokeAsync().ConfigureAwait(false);

        Assert.IsFalse(replay.CoverageStarted);
        Assert.DoesNotContain("Gathering coverage...", this.console.Output);
    }

    [TestMethod]
    public async Task ShouldWriteResultAndCoberturaFiles()
    {
        using var temp = new TempDirectory();
        var options = ScenarioCase.Get("Ok").ToOptions();
        options.Result = temp.File("result.xml");
        options.CcCobertura = temp.File("cobertura.xml");
        var scenario = Scenario.Load("Ok");
        scenario.JUnitXml = "<testsuites><testsuite id=\"1\" name=\"TestSchema\" tests=\"1\" errors=\"0\" failures=\"0\" /></testsuites>";
        var replay = new ReplayScenario(scenario);

        int exitCode = await this.CreateSut(options, replay).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeSuccess, exitCode);
        Assert.AreEqual(scenario.JUnitXml, await File.ReadAllTextAsync(options.Result).ConfigureAwait(false));
        Assert.Contains("<coverage lines-valid=\"5\" lines-covered=\"3\" ", await File.ReadAllTextAsync(options.CcCobertura).ConfigureAwait(false));
        Assert.AreEqual("JUnitXml Database.Tests", replay.SqlCalls[^1]);
    }

    [TestMethod]
    public async Task ShouldReportRunAllErrorAndStillShowResults()
    {
        var scenario = Scenario.Load("Fail");
        scenario.RunAllError = "Test Case Summary: 1 test case(s) executed, 0 succeeded, 0 skipped, 1 failed, 0 errored.";

        int exitCode = await this.CreateSut(ScenarioCase.Get("Fail").ToOptions(), new ReplayScenario(scenario)).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeTestsFailed, exitCode);
        Assert.Contains(scenario.RunAllError, this.console.Output);
        Assert.Contains("Gathering coverage...", this.console.Output);
    }

    [TestMethod]
    public async Task ShouldCountErroredTestsAsFailed()
    {
        var scenario = Scenario.Load("OkCcDisable");
        scenario.TestResults.Add(new TestResult() { Name = "[TestSchema].[test error]", Result = "Error", Msg = "Boom" });
        scenario.TestResults.Add(new TestResult() { Name = "[TestSchema].[test skipped]", Result = "Skipped" });

        int exitCode = await this.CreateSut(ScenarioCase.Get("OkCcDisable").ToOptions(), new ReplayScenario(scenario)).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeTestsFailed, exitCode);
        Assert.AreEqual("  [TestSchema].[test error]: Boom", this.console.Lines[^2]);
        Assert.That.IsLike(this.console.Lines[^1], "Failed: 1, Passed: 1, Duration: %");
    }

    [TestMethod]
    public async Task ShouldReportZeroCoverageWithoutStatements()
    {
        var scenario = Scenario.Load("Fail");
        scenario.Coverage = new CoverageReport(0, 0, [], string.Empty);

        _ = await this.CreateSut(ScenarioCase.Get("Fail").ToOptions(), new ReplayScenario(scenario)).InvokeAsync().ConfigureAwait(false);

        Assert.That.IsLike(this.console.Lines[^1], "Failed: 1, Passed: 0, Coverage: 0% (0/0), Duration: %");
    }

    [TestMethod]
    public async Task ShouldUseDatabaseFromExplicitTarget()
    {
        var options = ScenarioCase.Get("Ok").ToOptions();
        options.TargetConnectionString = "Server=myserver;User ID=u;Password=p;Initial Catalog=Custom";
        var replay = ReplayScenario.Load("Ok");
        var target = new ExplicitTarget(options.TargetConnectionString, replay);

        int exitCode = await this.CreateSut(options, replay, target).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeSuccess, exitCode);
        Assert.Contains("/p:TargetDatabaseName=Custom", replay.ProcessCalls[^1]);
        Assert.Contains("/p:TargetServerName=myserver", replay.ProcessCalls[^1]);
        Assert.Contains("RunAll Custom", replay.SqlCalls);
    }

    [TestMethod]
    public async Task ShouldUseProjectDatabaseWhenExplicitTargetHasNoCatalog()
    {
        var options = ScenarioCase.Get("Ok").ToOptions();
        options.TargetConnectionString = "Server=myserver;User ID=u;Password=p";
        var replay = ReplayScenario.Load("Ok");

        _ = await this.CreateSut(options, replay, new ExplicitTarget(options.TargetConnectionString, replay)).InvokeAsync().ConfigureAwait(false);

        Assert.Contains("RunAll Database.Tests", replay.SqlCalls);
    }

    [TestMethod]
    public async Task ShouldFailOnInvalidExplicitTarget()
    {
        var options = ScenarioCase.Get("Ok").ToOptions();
        options.TargetConnectionString = "Initial Catalog=Custom";

        int exitCode = await this.CreateSut(options, ReplayScenario.Load("Ok")).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeError, exitCode);
        Assert.Contains("must include Data Source", this.console.Output);
    }

    [TestMethod]
    public async Task ShouldFailOnMissingProject()
    {
        int exitCode = await this.CreateSut(new RunAllOptions() { Project = "does-not-exist" }, new ReplayScenario(new Scenario())).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeError, exitCode);
        Assert.AreEqual("Database project 'does-not-exist' does not exist.", this.console.Lines[^1]);
    }

    [TestMethod]
    public async Task ShouldTimeOutWhenPreparingTakesTooLong()
    {
        var options = ScenarioCase.Get("Ok").ToOptions();
        options.PrepareTimeout = TimeSpan.FromMilliseconds(10);
        var target = new FakeTargetProvider(async cancellationToken =>
        {
            await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
            return new RunTarget(FakeTargetProvider.ConnectionString);
        });

        int exitCode = await this.CreateSut(options, ReplayScenario.Load("Ok"), target).InvokeAsync().ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeError, exitCode);
        Assert.That.IsLike(this.console.Lines[^1], "Preparing database timed out after %");
    }

    [TestMethod]
    public async Task ShouldPropagateCancellation()
    {
        using var cts = new CancellationTokenSource();
        var target = new FakeTargetProvider(async cancellationToken =>
        {
            await cts.CancelAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return new RunTarget(FakeTargetProvider.ConnectionString);
        });

        var sut = this.CreateSut(ScenarioCase.Get("Ok").ToOptions(), ReplayScenario.Load("Ok"), target);

        _ = await Assert.ThrowsAsync<OperationCanceledException>(() => sut.InvokeAsync(cts.Token)).ConfigureAwait(false);
    }

    private RunAllCommand CreateSut(RunAllOptions options, ReplayScenario replay, ITargetProvider? target = null)
    {
        return new RunAllCommand(options, target ?? new FakeTargetProvider(), new DotnetTool(replay), replay, replay, this.console, this.time);
    }
}
