namespace SqlTest.DockerTests;

[TestClass]
[TestCategory("Docker")]
[DoNotParallelize]
public class OkDockerTests : DockerTestBase
{
    [TestMethod]
    public async Task ShouldRunOk()
    {
        await RunScenarioAsync("Ok").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ShouldRunOkWithCodeCoverageIncludeTsqlt()
    {
        await RunScenarioAsync("OkIncludeTsqlt").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ShouldRunOkWithCodeCoverageDisable()
    {
        await RunScenarioAsync("OkCcDisable").ConfigureAwait(false);
    }
}
