namespace SqlTest.Tests;

[TestClass]
public class ErrorDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllError()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Error");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%error MSB3073%");
        Assert.That.IsLike(results.Last().ToString(), "Deploying database failed.");
    }
}