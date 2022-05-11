namespace SqlTest.Tests;

using LikeComparison.TransactSql;

[TestClass]
public class DatabaseTests
{
    private readonly string image =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/azure-sql-edge";
#endif

    [TestMethod]
    public void InvokeSqlTestRunAllOk()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Ok");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, Coverage: 60% (3/5)");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllOkWithIncludeTsqlt()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Ok --cc-include-tsqlt");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, Coverage: __[%] (%/%)");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllFail()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Fail");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%failed%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllError()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Error");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%error MSB3073%");
        Assert.That.IsLike(results.Last().ToString(), "Deploying database failed.");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllException()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Exception");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllCollationPassed()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Collation --collation Polish_CI_AS");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllCollationFailed()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Collation");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%Cannot resolve the collation conflict%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }
}