namespace SqlTest.Tests;

[TestClass]
public class OkDatabaseTests : DatabaseTests
{
    [TestMethod]
    public void InvokeSqlTestRunAllOk()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Ok");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, Coverage: 60% (3/5), Duration: %");
    }

    [TestMethod]
    public void InvokeSqlTestRunAllOkWithIncludeTsqlt()
    {
        var results = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Ok --cc-include-tsqlt");

        Assert.That.IsLike(results.Last().ToString(), "Failed: 0, Passed: 1, Coverage: __[%] (%/%), Duration: %");
    }
}