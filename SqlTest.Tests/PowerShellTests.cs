namespace SqlTest.Tests;

using LikeComparison.TransactSql;

[TestClass]
public class PowerShellTests
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
    public void RunScriptGetImage()
    {
        string image =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/mssql/server:2019-latest";
#endif

        _ = PowerShellExtensions.RunScript($"dotnet SqlTest.dll --image {image}");
    }
}