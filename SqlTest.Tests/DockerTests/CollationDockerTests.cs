namespace SqlTest.DockerTests;

[TestClass]
[TestCategory("Docker")]
[DoNotParallelize]
public class CollationDockerTests : DockerTestBase
{
    [TestMethod]
    public async Task ShouldPassWithServerCollation()
    {
        await RunScenarioAsync("CollationPassed").ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ShouldFailWithDefaultCollation()
    {
        await RunScenarioAsync("CollationFailed").ConfigureAwait(false);
    }
}
