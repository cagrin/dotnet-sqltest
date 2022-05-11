namespace SqlTest.Tests;

[TestClass]
public class ExceptionDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllException()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Exception");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }
}