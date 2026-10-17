namespace SqlTest.UnitTests;

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
    public async Task ShouldDisposeWithoutStart()
    {
        var tc = new MsSqlTestcontainer(image: null, collation: null);

        await tc.DisposeAsync().ConfigureAwait(false);
    }
}