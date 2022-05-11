namespace SqlTest.Tests;

[TestClass]
public class PowerShellCommandTests
{
    [TestMethod]
    public void InvokeDotnetError()
    {
        _ = PowerShellCommand.Invoke("dotnet --error");
    }

    [TestMethod]
    public void InvokeSqlTestVersion()
    {
        var results = PowerShellCommand.Invoke("dotnet SqlTest.dll --version");

        Assert.That.IsLike(results.First().ToString(), "%.%.%+%");
    }

    [TestMethod]
    public void InvokeSqlTestHelp()
    {
        var results = PowerShellCommand.Invoke("dotnet SqlTest.dll --help");

        Assert.That.IsLike(results.First().ToString(), "Description:");
    }
}