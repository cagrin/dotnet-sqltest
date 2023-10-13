namespace SqlTest.Tests;

[TestClass]
public class MsSqlFactoryTests
{
    [TestMethod]
    public async Task ShouldBuildWindowsContainer()
    {
        var tc = MsSqlFactory.CreateTestcontainer(image: "mcr.microsoft.com/mssql/server", collation: string.Empty);
        await tc.DisposeAsync().ConfigureAwait(false);
    }
}