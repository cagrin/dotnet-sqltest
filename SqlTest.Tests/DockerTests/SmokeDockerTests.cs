namespace SqlTest.DockerTests;

// Runs the built SqlTest.dll as a child process, the same way users run the tool.
[TestClass]
[TestCategory("Docker")]
[DoNotParallelize]
public class SmokeDockerTests : DockerTestBase
{
    [TestMethod]
    public async Task ShouldRunToolEndToEnd()
    {
        await EnsureImagePulledAsync().ConfigureAwait(false);
        using var temp = new TempDirectory();
        string result = temp.File("result.xml");
        string cobertura = temp.File("cobertura.xml");

        var process = await new ProcessRunner().RunAsync(
            DotnetTool.FileName,
            ["SqlTest.dll", "runall", "--image", Image, "--project", Path.Combine(TestPaths.DatabaseTests, "Ok"), "--result", result, "--cc-cobertura", cobertura],
            CancellationToken.None).ConfigureAwait(false);

        ScenarioCase.Get("Ok").Verify(process.ExitCode, process.StandardOutput);

        Assert.That.IsLike(await File.ReadAllTextAsync(result).ConfigureAwait(false), """<testsuites><testsuite id="1" name="TestSchema" tests="1" errors="0" failures="0" skipped="0" timestamp="____-__-__T__:__:__" time="_.____" hostname="%" package="tSQLt"><properties /><testcase classname="TestSchema" name="test that this test is ok" time="_.____" /><system-out /><system-err /></testsuite></testsuites>""");

        string coberturaPattern = """
<?xml version="1.0"?>
<!--DOCTYPE coverage SYSTEM "http://cobertura.sourceforge.net/xml/coverage-03.dtd"-->
<coverage lines-valid="5" lines-covered="3" line-rate="0[.,]6" version="1[.,]9" timestamp="__________[.,]%">
 <packages>
  <package name="Database.Tests">
    <classes>
    <class name="[[]dbo].[[]Assertions]" filename="[[]dbo].[[]Assertions]" lines-valid="3" lines-covered="3" line-rate="1" >
     <methods/>
     <lines>
      <line number="5" hits="1" branch="false" />
      <line number="7" hits="1" branch="false" />
      <line number="9" hits="1" branch="false" />
     </lines>
    </class>
    <class name="[[]dbo].[[]Example]" filename="[[]dbo].[[]Example]" lines-valid="2" lines-covered="0" line-rate="0" >
     <methods/>
     <lines>
      <line number="5" hits="0" branch="false" />
      <line number="7" hits="0" branch="false" />
      <line number="8" hits="0" branch="false" />
     </lines>
    </class>

   </classes>
  </package>
 </packages>
</coverage>%
""";

        Assert.That.IsLike((await File.ReadAllTextAsync(cobertura).ConfigureAwait(false)).ReplaceLineEndings("\n"), coberturaPattern.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public async Task ShouldRunAgainstExplicitTarget()
    {
        await EnsureImagePulledAsync().ConfigureAwait(false);
        await using var container = new MsSqlTestcontainer(Image, collation: null);
        var target = await container.StartAsync(CancellationToken.None).ConfigureAwait(false);

        var options = ScenarioCase.Get("Ok").ToOptions();
        options.TargetConnectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(target.TargetConnectionString) { InitialCatalog = "ExplicitTarget" }.ConnectionString;

        int exitCode = await Program.InvokeRunAllAsync(options, CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual(RunAllCommand.ExitCodeSuccess, exitCode);
    }
}
