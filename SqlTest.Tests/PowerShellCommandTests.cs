#if DEBUG
[assembly: Parallelize(Workers = 8, Scope = ExecutionScope.MethodLevel)]
#endif

namespace SqlTest.Tests;

using LikeComparison.TransactSql;

[TestClass]
public class PowerShellCommandTests
{
    private readonly string image =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/azure-sql-edge";
#endif

    [TestMethod]
    public void InvokeDotnetError()
    {
        _ = new PowerShellCommand().Invoke("dotnet --error");
    }

    [TestMethod]
    public void InvokeSqlTestVersion()
    {
        var results = new PowerShellCommand().Invoke("dotnet SqlTest.dll --version");

        Assert.That.IsLike(results.First().ToString(), "%.%.%+%");
    }

    [TestMethod]
    public void InvokeSqlTestHelp()
    {
        var results = new PowerShellCommand().Invoke("dotnet SqlTest.dll --help");

        Assert.That.IsLike(results.First().ToString(), "Description:");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllOk()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Ok");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, Coverage: 60% (3/5)");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllFail()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Fail");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%failed%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllError()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Error");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%error MSB3073%");
        Assert.That.IsLike(results.Last().ToString(), "Deploying database failed.");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllException()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Exception");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllCollationPassed()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Collation --collation Polish_CI_AS");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllCollationFailed()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Collation");

        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "%Cannot resolve the collation conflict%");
        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, %");
    }
}