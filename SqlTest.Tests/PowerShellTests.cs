namespace SqlTest.Tests;

[TestClass]
public class PowerShellTests
{
    [TestMethod]
    public void RunScript()
    {
        PowerShellExtensions.RunScript("dotnet --version");
    }

    [TestMethod]
    public void RunScriptError()
    {
        PowerShellExtensions.RunScript("dotnet --error");
    }
}