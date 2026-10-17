namespace SqlTest.DockerTests;

[TestClass]
[TestCategory("Docker")]
[DoNotParallelize]
public class FailDockerTests : DockerTestBase
{
    [TestMethod]
    public async Task ShouldRunFail()
    {
        await RunScenarioAsync("Fail").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ShouldRunException()
    {
        await RunScenarioAsync("Exception").ConfigureAwait(false);
    }
}
