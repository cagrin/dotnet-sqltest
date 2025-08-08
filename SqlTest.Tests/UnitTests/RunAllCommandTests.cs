namespace SqlTest.Tests.UnitTests;

[TestClass]
public class RunAllCommandTests
{
    [TestMethod]
    public void ShouldRunDispose()
    {
        var mock = new MockIConsole();

        using var stc = new RunAllCommand(new RunAllOptions(), mock.Object);
    }

    [TestMethod]
    public void InvokeSqlTestRunAll()
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --help");
        var expected =
"""
Description:
  Run all tests.

Usage:
  SqlTest runall [options]

Options:
  -i, --image         Docker image.
  -p, --project       Database project.
  -c, --collation     Server collation.
  -r, --result        Save result to JUnit XML file.
  --cc-cobertura      Save code coverage to Cobertura XML file.
  --cc-disable        Disable code coverage.
  --cc-include-tsqlt  Include code coverage of tSQLt schema.
  -?, -h, --help      Show help and usage information

""";

        results.ShouldBeEquivalentTo(expected.Split("\n"));
    }
}