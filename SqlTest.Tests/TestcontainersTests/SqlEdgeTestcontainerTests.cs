namespace SqlTest.Tests;

[TestClass]
public class SqlEdgeTestcontainerTests
{
    [TestMethod]
    public async Task ShouldStartAsyncAndDispose()
    {
        var tc = new SqlEdgeTestcontainer();
        _ = await tc.StartAsync(image: "mcr.microsoft.com/azure-sql-edge", collation: "SQL_Latin1_General_CP1_CI_AS").ConfigureAwait(false);
        tc.Dispose();
    }

    [TestMethod]
    public void ShouldDispose()
    {
        var tc = new SqlEdgeTestcontainer();
        tc.Dispose();
    }
}