namespace SqlTest.Tests;

using System.Management.Automation;
using LikeComparison.TransactSql;

[TestClass]
public class PowerShellTests
{
    [TestMethod]
    public async Task RunScript()
    {
        using var ps = PowerShell.Create();

        _ = ps.AddScript("dotnet --version");

        var psobjs = await ps.InvokeAsync().ConfigureAwait(false);

        var psout = psobjs.First().BaseObject.ToString();

        Assert.IsTrue(psout?.Like("6.%"));
    }
}