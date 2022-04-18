namespace SqlTest.Tests;

using LikeComparison.TransactSql;

[TestClass]
public class PowerShellCommandTests
{
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
    public void InvokeSqlTestRunAll()
    {
        string image =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/azure-sql-edge";
#endif

        var results = new PowerShellCommand().Invoke($"dotnet SqlTest.dll runall --image {image} --project ../../../../Database.Tests");

        Assert.That.IsLike(results.Last().ToString(), "%Running all tests%");
    }
}