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
        var filename = $"result.xml";
        _ = PowerShellCommand.Invoke($"dotnet SqlTest.dll runall --image {this.Image} --project ../../../../Database.Tests/Ok --result {filename}");

        using var str = new StreamReader(filename);
        string xml = str.ReadToEnd();
        string pattern = """<testsuites><testsuite id="1" name="TestSchema" tests="1" errors="0" failures="0" skipped="0" timestamp="____-__-__T__:__:__" time="_.____" hostname="%" package="tSQLt"><properties /><testcase classname="TestSchema" name="test that this test is ok" time="_.____" /><system-out /><system-err /></testsuite></testsuites>""";

        Assert.That.IsLike(xml, pattern);
    }
}