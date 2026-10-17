namespace SqlTest.DockerTests;

[TestClass]
[TestCategory("Docker")]
[DoNotParallelize]
public class ProjectReferencesDockerTests : DockerTestBase
{
    [TestMethod]
    public async Task ShouldDeployReferencedDatabases()
    {
        await RunScenarioAsync("ProjectReferences").ConfigureAwait(false);
    }
}
