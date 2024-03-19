namespace SqlTest.DatabaseTests;

[TestClass]
public class OkDatabaseTests : BaseDatabaseTests
{
    public static new IEnumerable<object[]> Images => BaseDatabaseTests.Images;

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllOk(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Ok; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: 60% (3/5), Duration: %");
    }

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllOkWithCodeCoverageIncludeTsqlt(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Ok --cc-include-tsqlt; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: __[%] (%/%), Duration: %");
    }

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllOkWithCodeCoverageDisable(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Ok --cc-disable; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Duration: %");
    }

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllOkWithXmlResult(string image)
    {
        var filename = $"result.xml";
        _ = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Ok --result {filename}");

        using var str = new StreamReader(filename);
        string xml = str.ReadToEnd();
        string pattern = """<testsuites><testsuite id="1" name="TestSchema" tests="1" errors="0" failures="0" skipped="0" timestamp="____-__-__T__:__:__" time="_.____" hostname="%" package="tSQLt"><properties /><testcase classname="TestSchema" name="test that this test is ok" time="_.____" /><system-out /><system-err /></testsuite></testsuites>""";

        Assert.That.IsLike(xml, pattern);
    }
}