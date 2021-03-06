namespace SqlTest.Tests;

[TestClass]
public class FailDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllFail()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Fail\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "1");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 1, Passed: 0, %");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%failed%");
    }
}