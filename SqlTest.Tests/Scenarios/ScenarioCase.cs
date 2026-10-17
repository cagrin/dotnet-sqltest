namespace SqlTest.Tests;

// Expected outcome of a scenario, shared by replay unit tests and tests against real infrastructure.
internal sealed record ScenarioCase(string Name, string Project, int ExitCode, string LastLine, string? PreviousLine = null)
{
    private static readonly ScenarioCase[] Cases =
    [
        new("Ok", "Database.Tests/Ok", 0, "Failed: 0, Passed: 1, Coverage: 60% (3/5), Duration: %", "  [[]dbo].[[]Example]: UPDATE #Example SET [[]...]"),
        new("OkIncludeTsqlt", "Database.Tests/Ok", 0, "Failed: 0, Passed: 1, Coverage: __[%] (%/%), Duration: %") { CcIncludeTsqlt = true },
        new("OkCcDisable", "Database.Tests/Ok", 0, "Failed: 0, Passed: 1, Duration: %") { CcDisable = true },
        new("Fail", "Database.Tests/Fail", 1, "Failed: 1, Passed: 0, %", "%failed%"),
        new("Exception", "Database.Tests/Exception", 1, "Failed: 1, Passed: 0, %", "%Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements%"),
        new("Error", "Database.Tests/Error", 2, "Deploying database failed.", "%error MSB3073%"),
        new("CollationPassed", "Database.Tests/Collation", 0, "Failed: 0, Passed: 1, %") { Collation = "Polish_CI_AS" },
        new("CollationFailed", "Database.Tests/Collation", 1, "Failed: 1, Passed: 0, %", "%Cannot resolve the collation conflict%"),
        new("ProjectReferences", "ProjectReferences.Tests/Test", 0, "Failed: 0, Passed: 1, Coverage: 100% (3/3), Duration: %"),
    ];

    public string? Collation { get; init; }

    public bool CcDisable { get; init; }

    public bool CcIncludeTsqlt { get; init; }

    public static IEnumerable<object[]> Names => Cases.Select(p => new object[] { p.Name });

    public static ScenarioCase Get(string name)
    {
        return Cases.Single(p => p.Name == name);
    }

    public RunAllOptions ToOptions()
    {
        return new RunAllOptions()
        {
            Project = Path.Combine(TestPaths.RepositoryRoot, this.Project),
            Collation = this.Collation,
            CcDisable = this.CcDisable,
            CcIncludeTsqlt = this.CcIncludeTsqlt,
        };
    }

    public void Verify(int exitCode, IReadOnlyList<string> lines)
    {
        string output = string.Join(Environment.NewLine, lines);

        Assert.AreEqual(this.ExitCode, exitCode, output);
        Assert.IsTrue(lines[^1].Like(this.LastLine), $"Last line is not like '{this.LastLine}':{Environment.NewLine}{output}");

        if (this.PreviousLine != null)
        {
            Assert.IsTrue(lines[^2].Like(this.PreviousLine), $"Previous line is not like '{this.PreviousLine}':{Environment.NewLine}{output}");
        }
    }
}
