namespace SqlTest.Tests;

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
    }
}