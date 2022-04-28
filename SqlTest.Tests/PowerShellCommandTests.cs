[assembly: Parallelize(Workers = 8, Scope = ExecutionScope.MethodLevel)]

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

        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, Coverage: 0% (0/0)");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllError()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Error");

        Assert.That.IsLike(results.Last().ToString(), "Deploying database failed.");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllException()
    {
        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {this.image} --project ../../../../Database.Tests/Exception");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 1, Passed: 0, Coverage: 0% (0/0)");
    }
}