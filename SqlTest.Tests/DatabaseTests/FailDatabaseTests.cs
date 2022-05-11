namespace SqlTest.Tests;

[TestClass]
public class FailDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllFail()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Fail");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%failed%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }
}