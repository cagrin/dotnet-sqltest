namespace SqlTest.Tests;

using LikeComparison.TransactSql;

[TestClass]
public class PowerShellExtensionsTests
{
    [TestMethod]
    public void RunScriptError()
    {
        _ = PowerShellExtensions.RunScript("dotnet --error");
    }

    [TestMethod]
    public void RunScriptGetVersion()
    {
        var results = PowerShellExtensions.RunScript("dotnet SqlTest.dll --version");

        Assert.That.IsLike(results.First().ToString(), "%.%.%+%");
    }

    [TestMethod]
    public void RunScriptGetHelp()
    {
        var results = PowerShellExtensions.RunScript("dotnet SqlTest.dll --help");

        Assert.That.IsLike(results.First().ToString(), "Description:");
    }

    [TestMethod]
    public void RunScriptGetInvokeProject()
    {
        string image =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/azure-sql-edge";
#endif

        var results = PowerShellExtensions.RunScript($"dotnet SqlTest.dll --image {image} --project ../../../../Database.Tests");

        Assert.That.IsLike(results.Last().ToString(), "%Successfully deployed database%");
    }
}