namespace SqlTest.IntegrationTests;

// The Error project fails to build, so the whole flow runs against a real database project without any SQL Server.
[TestClass]
[TestCategory("Integration")]
public class ErrorScenarioTests
{
    [TestMethod]
    public async Task ShouldFailToDeployRealProjectWithBuildError()
    {
        RecordingScenario.UseToolCulture();

        var scenario = ScenarioCase.Get("Error");
        var console = new FakeConsole();
        var recording = new RecordingScenario();

        var sut = new RunAllCommand(scenario.ToOptions(), new FakeTargetProvider(), new DotnetTool(recording), recording, recording, console, TimeProvider.System);

        int exitCode = await sut.InvokeAsync().ConfigureAwait(false);

        scenario.Verify(exitCode, console.Lines);
        Assert.AreNotEqual(0, recording.Scenario.Processes.Single(p => p.Command == "build").Result.ExitCode);

        recording.SaveIfRequested(scenario.Name);
    }
}
