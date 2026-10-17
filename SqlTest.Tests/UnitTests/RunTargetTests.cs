namespace SqlTest.UnitTests;

[TestClass]
public class RunTargetTests
{
    [TestMethod]
    public void ShouldParseIntegratedSecurityTarget()
    {
        var sut = new RunTarget("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=SSPI;Initial Catalog=Database.Tests");

        Assert.AreEqual("(localdb)\\MSSQLLocalDB", sut.TargetServerName);
        Assert.AreEqual(0, sut.TargetPort);
        Assert.AreEqual(string.Empty, sut.TargetUser);
        Assert.AreEqual(string.Empty, sut.TargetPassword);
        Assert.AreEqual("Database.Tests", sut.TargetDatabaseName);
        Assert.DoesNotContain("Database.Tests", sut.TargetConnectionString);
    }

    [TestMethod]
    public void ShouldParseSqlAuthTargetWithPort()
    {
        var sut = new RunTarget("Server=localhost,1433;User ID=sa;Password=StrongPassword!;Initial Catalog=Database.Tests");

        Assert.AreEqual("localhost", sut.TargetServerName);
        Assert.AreEqual(1433, sut.TargetPort);
        Assert.AreEqual("sa", sut.TargetUser);
        Assert.AreEqual("StrongPassword!", sut.TargetPassword);
        Assert.AreEqual("Database.Tests", sut.TargetDatabaseName);
    }

    [TestMethod]
    public void ShouldFallbackToIntegratedSecurityWhenCredentialsMissing()
    {
        var sut = new RunTarget("Data Source=(local)");

        Assert.AreEqual("(local)", sut.TargetServerName);
        Assert.AreEqual(string.Empty, sut.TargetUser);
        Assert.AreEqual(string.Empty, sut.TargetPassword);
        Assert.That.IsLike(sut.TargetConnectionString, "%Integrated Security=True%");
        Assert.That.IsLike(sut.TargetConnectionString, "%Trust Server Certificate=True%");
    }

    [TestMethod]
    public void ShouldKeepServerNameWhenPortIsNotNumeric()
    {
        var sut = new RunTarget("Server=host,abc;User ID=sa;Password=x");

        Assert.AreEqual("host,abc", sut.TargetServerName);
        Assert.AreEqual(0, sut.TargetPort);
    }

    [TestMethod]
    public void ShouldThrowWhenDataSourceIsMissing()
    {
        _ = Assert.Throws<ArgumentException>(() => new RunTarget("Initial Catalog=Database.Tests"));
    }
}