namespace SqlTest.Tests;

[TestClass]
public class CollationDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllCollationPassed()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Collation --collation Polish_CI_AS\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllCollationFailed()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Collation\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "1");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 1, Passed: 0, %");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%Cannot resolve the collation conflict%");
    }
}