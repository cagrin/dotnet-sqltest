namespace SqlTest.UnitTests;

using System.CommandLine;

[TestClass]
public class ProgramTests
{
    private readonly StringWriter output = new StringWriter();

    private readonly StringWriter error = new StringWriter();

    private RunAllOptions? captured;

    [TestMethod]
    public async Task ShouldShowHelp()
    {
        int exitCode = await this.RunAsync("runall", "--help").ConfigureAwait(false);

        string expected = """
            Description:
              Run all tests

            Usage:
              SqlTest runall [options]

            Options:
              -p, --project <project> (REQUIRED)  Database project
              -i, --image <image>                 Docker image
              -c, --collation <collation>         Server collation
              -t, --target <target>               Target connection string (mutually exclusive with --image)
              -r, --result <result>               Save result to JUnit XML file
              --cc-cobertura <cc-cobertura>       Save code coverage to Cobertura XML file
              --cc-disable                        Disable code coverage
              --cc-include-tsqlt                  Include code coverage of tSQLt schema
              -?, -h, --help                      Show help and usage information
            """;

        Assert.AreEqual(0, exitCode);
        Assert.AreEqual(Normalize(expected), Normalize(this.output.ToString().Replace("testhost", "SqlTest", StringComparison.Ordinal)));
        Assert.IsNull(this.captured);
    }

    [TestMethod]
    public async Task ShouldPassAllOptions()
    {
        int exitCode = await this.RunAsync("runall", "-p", "proj", "-i", "img", "-c", "Polish_CI_AS", "-r", "r.xml", "--cc-cobertura", "c.xml", "--cc-disable", "--cc-include-tsqlt").ConfigureAwait(false);

        Assert.AreEqual(42, exitCode);
        Assert.IsNotNull(this.captured);
        Assert.AreEqual("proj", this.captured.Project);
        Assert.AreEqual("img", this.captured.Image);
        Assert.AreEqual("Polish_CI_AS", this.captured.Collation);
        Assert.AreEqual("r.xml", this.captured.Result);
        Assert.AreEqual("c.xml", this.captured.CcCobertura);
        Assert.IsTrue(this.captured.CcDisable);
        Assert.IsTrue(this.captured.CcIncludeTsqlt);
        Assert.IsFalse(this.captured.UseExplicitTarget);
    }

    [TestMethod]
    public async Task ShouldPassTarget()
    {
        _ = await this.RunAsync("runall", "-p", "proj", "-t", "Data Source=(local)").ConfigureAwait(false);

        Assert.IsNotNull(this.captured);
        Assert.IsTrue(this.captured.UseExplicitTarget);
        Assert.AreEqual("Data Source=(local)", this.captured.TargetConnectionString);
    }

    [TestMethod]
    public async Task ShouldRejectImageWithTarget()
    {
        int exitCode = await this.RunAsync("runall", "-p", "proj", "-i", "img", "-t", "Data Source=(local)").ConfigureAwait(false);

        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("mutually exclusive", this.error.ToString());
        Assert.IsNull(this.captured);
    }

    [TestMethod]
    public async Task ShouldRequireProject()
    {
        int exitCode = await this.RunAsync("runall").ConfigureAwait(false);

        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("--project", this.error.ToString());
        Assert.IsNull(this.captured);
    }

    [TestMethod]
    public async Task ShouldCreateTargetProvider()
    {
        await using var container = Program.CreateTargetProvider(new RunAllOptions() { Image = "img" }, new SqlServerClient());
        await using var explicitTarget = Program.CreateTargetProvider(new RunAllOptions() { TargetConnectionString = "Data Source=(local)" }, new SqlServerClient());

        Assert.IsInstanceOfType<MsSqlTestcontainer>(container);
        Assert.IsInstanceOfType<ExplicitTarget>(explicitTarget);
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldComposeRealDependenciesAndFailFastOnMissingProject()
    {
        var original = Console.Out;
        using var console = new StringWriter();
        Console.SetOut(console);

        try
        {
            int exitCode = await Program.InvokeRunAllAsync(new RunAllOptions() { Project = "does-not-exist" }, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(RunAllCommand.ExitCodeError, exitCode);
        }
        finally
        {
            Console.SetOut(original);
        }

        Assert.Contains("Database project 'does-not-exist' does not exist.", console.ToString());
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task ShouldRunMain()
    {
        var original = Console.Out;
        using var console = new StringWriter();
        Console.SetOut(console);

        try
        {
            int exitCode = await Program.Main(["--help"]).ConfigureAwait(false);

            Assert.AreEqual(0, exitCode);
        }
        finally
        {
            Console.SetOut(original);
        }

        Assert.Contains("runall", console.ToString());
    }

    private static string Normalize(string text)
    {
        return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
    }

    private Task<int> RunAsync(params string[] args)
    {
        return Program.RunAsync(
            args,
            (options, cancellationToken) =>
            {
                this.captured = options;
                return Task.FromResult(42);
            },
            new InvocationConfiguration() { Output = this.output, Error = this.error });
    }
}
