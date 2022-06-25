namespace SqlTest.Tests;

[TestClass]
public class PowerShellCommandTests
{
    [TestMethod]
    public void InvokeSqlTestError()
    {
        var mock = new MockIConsole();

        _ = PowerShellCommand.Invoke("dotnet SqlTest.dll --error", mock.Object);

        // Assert.That.IsLike(mock.Output, "Required command was not provided.%Unrecognized command or argument '--error'.%");
        Assert.IsTrue(mock.Output.StartsWith("Required command was not provided.\nUnrecognized command or argument '--error'.", StringComparison.CurrentCulture));
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