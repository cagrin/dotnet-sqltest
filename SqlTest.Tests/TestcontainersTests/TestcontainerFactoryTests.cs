namespace SqlTest.Tests;

[TestClass]
public class TestcontainerFactoryTests
{
    [TestMethod]
    public void ShouldCreateLocalhostTestcontainer()
    {
        var testcontainer = TestcontainerFactory.Create(null!);

        Assert.IsInstanceOfType(testcontainer, typeof(LocalhostTestcontainer));
    }

    [TestMethod]
    public void ShouldCreateMsSqlTestcontainer()
    {
        var testcontainer = TestcontainerFactory.Create("mcr.microsoft.com/mssql/server");

        Assert.IsInstanceOfType(testcontainer, typeof(MsSqlTestcontainer));
    }

    [DataTestMethod]
    [DataRow("SQL_Latin1_General_CP1_CI_AS", null)]
    [DataRow("SQL_Latin1_General_CP1_CI_AS", "")]
    [DataRow("SQL_Latin1_General_CP1_CI_AS", "SQL_Latin1_General_CP1_CI_AS")]
    [DataRow("Latin1_General_CI_AS", "Latin1_General_CI_AS")]
    public void ShouldSetCollation(string expected, string collation)
    {
        string actual = TestcontainerFactory.WithCollation(collation);

        Assert.AreEqual(expected, actual);
    }
}