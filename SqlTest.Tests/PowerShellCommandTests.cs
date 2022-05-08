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
}