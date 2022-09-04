namespace SqlTest.DatabaseTests;

[TestClass]
public class ErrorDatabaseTests : BaseDatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllError()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Error\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "2");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Deploying database failed.");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%error MSB3073%");
    }
}