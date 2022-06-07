namespace SqlTest.Tests;

[TestClass]
public class OkDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllOk()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Ok\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: 60% (3/5), Duration: %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllOkWithIncludeTsqlt()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Ok --cc-include-tsqlt\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: __[%] (%/%), Duration: %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllOkWithXmlResult()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Ok --result OkResults.xml\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: 60% (3/5), Duration: %");
    }
}