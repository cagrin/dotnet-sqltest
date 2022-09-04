namespace SqlTest.DatabaseTests;

[TestClass]
public class ExceptionDatabaseTests : BaseDatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllException()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Exception\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "1");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 1, Passed: 0, %");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements%");
    }
}