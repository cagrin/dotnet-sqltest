namespace SqlTest.Tests;

[TestClass]
public class MsSqlFactoryTests
{
    [TestMethod]
    public async Task ShouldBuildWindowsContainer()
    {
        var tc = MsSqlFactory.CreateTestcontainer(image: "cagrin/mssql-server-ltsc2022", collation: string.Empty, windowsContainer: true);
        await tc.DisposeAsync().ConfigureAwait(false);
    }
}