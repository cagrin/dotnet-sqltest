namespace SqlTest.Tests;

[TestClass]
public class MsSqlTestcontainerTests
{
    [TestMethod]
    public async Task ShouldStartAsyncAndDispose()
    {
        var tc = new MsSqlTestcontainer();
        _ = await tc.StartAsync(image: "mcr.microsoft.com/mssql/server", collation: "SQL_Latin1_General_CP1_CI_AS").ConfigureAwait(false);
        tc.Dispose();
    }

    [TestMethod]
    public void ShouldDispose()
    {
        var tc = new MsSqlTestcontainer();
        tc.Dispose();
    }
}