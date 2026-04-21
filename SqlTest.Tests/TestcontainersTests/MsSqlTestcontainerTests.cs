namespace SqlTest.Tests;

[TestClass]
public class MsSqlTestcontainerTests
{
    [TestMethod]
    [DataRow("SQL_Latin1_General_CP1_CI_AS", null)]
    [DataRow("SQL_Latin1_General_CP1_CI_AS", "")]
    [DataRow("SQL_Latin1_General_CP1_CI_AS", "SQL_Latin1_General_CP1_CI_AS")]
    [DataRow("Latin1_General_CI_AS", "Latin1_General_CI_AS")]
    public void ShouldSetCollation(string expected, string collation)
    {
        string actual = MsSqlTestcontainer.WithCollation(collation);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task ShouldStartAsyncAndDispose()
    {
        var tc = new MsSqlTestcontainer();
        _ = await tc.StartAsync(image: "mcr.microsoft.com/mssql/server:2019-latest", collation: "SQL_Latin1_General_CP1_CI_AS").ConfigureAwait(false);
        tc.Dispose();
    }

    [TestMethod]
    public void ShouldDispose()
    {
        var tc = new MsSqlTestcontainer();
        tc.Dispose();
    }
}