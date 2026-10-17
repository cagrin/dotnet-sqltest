namespace SqlTest.IntegrationTests;

[TestClass]
[TestCategory("Integration")]
public class ProcessRunnerTests
{
    [TestMethod]
    public async Task ShouldCaptureOutputAndExitCode()
    {
        var echo = new FakeConsole();

        var result = await new ProcessRunner(echo).RunAsync(DotnetTool.FileName, ["--version"], CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotEmpty(result.StandardOutput);
        Assert.Contains(result.StandardOutput[0], echo.Output);
    }

    [TestMethod]
    public async Task ShouldPassArgumentsWithoutShellInterpretation()
    {
        var result = await new ProcessRunner().RunAsync(DotnetTool.FileName, ["this-command-does-not-exist; $(echo) \"quoted\""], CancellationToken.None).ConfigureAwait(false);

        Assert.AreNotEqual(0, result.ExitCode);
        Assert.Contains("this-command-does-not-exist; $(echo)", string.Join('\n', result.StandardOutput.Concat(result.StandardError)));
    }

    [TestMethod]
    public async Task ShouldKillProcessOnCancellation()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        _ = await Assert.ThrowsAsync<OperationCanceledException>(() => new ProcessRunner().RunAsync(DotnetTool.FileName, ["--info"], cts.Token)).ConfigureAwait(false);
    }
}
