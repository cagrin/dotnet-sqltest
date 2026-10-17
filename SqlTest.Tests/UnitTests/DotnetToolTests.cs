namespace SqlTest.UnitTests;

[TestClass]
public class DotnetToolTests
{
    [TestMethod]
    public void ShouldGetPublishArgumentsWithSqlAuthentication()
    {
        var target = new RunTarget("Server=localhost,1433;User ID=sa;Password='Str0ng;Pa$$,word'");

        var sut = DotnetTool.GetPublishArguments("{project}", target, "{database}");

        Assert.AreSequenceEqual(
            new[]
            {
                "publish",
                "{project}",
                "/t:PublishDatabase",
                "/p:TargetDatabaseName={database}",
                "/p:TargetServerName=localhost",
                "/p:TargetPort=1433",
                "/p:TargetUser=sa",
                "/p:TargetPassword=Str0ng%3BPa%24%24%2Cword",
                "/p:CreateNewDatabase=true",
            },
            sut);
    }

    [TestMethod]
    public void ShouldGetPublishArgumentsWithIntegratedSecurity()
    {
        var target = new RunTarget("Data Source=(localdb)\\MSSQLLocalDB");

        var sut = DotnetTool.GetPublishArguments("{project}", target, string.Empty);

        Assert.AreSequenceEqual(
            new[] { "publish", "{project}", "/t:PublishDatabase", "/p:TargetServerName=(localdb)\\MSSQLLocalDB", "/p:CreateNewDatabase=true" },
            sut);
    }

    [TestMethod]
    [DataRow("plain", "plain")]
    [DataRow("%$@';,?*", "%25%24%40%27%3B%2C%3F%2A")]
    [DataRow("\"quoted value\"", "\"quoted value\"")]
    public void ShouldEscapeMsBuildProperty(string value, string expected)
    {
        Assert.AreEqual(expected, DotnetTool.EscapeProperty(value));
    }

    [TestMethod]
    public void ShouldGetCleanAndBuildArguments()
    {
        Assert.AreSequenceEqual(new[] { "clean", "{project}" }, DotnetTool.GetCleanArguments("{project}"));
        Assert.AreSequenceEqual(new[] { "build", "{project}" }, DotnetTool.GetBuildArguments("{project}"));
    }

    [TestMethod]
    public async Task ShouldCleanThenBuild()
    {
        var replay = ReplayScenario.Load("Ok");

        var result = await new DotnetTool(replay).CleanBuildAsync("Database.Tests.csproj", CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual(0, result.ExitCode);
        Assert.AreSequenceEqual(new[] { "clean", "build" }, replay.ProcessCalls.Select(p => p[0]));
    }

    [TestMethod]
    public async Task ShouldPublishReferencedDatabasesFirst()
    {
        var replay = ReplayScenario.Load("ProjectReferences");
        var project = SqlProject.Load(Path.Combine(TestPaths.ProjectReferencesTests, "Test"));
        var target = new RunTarget(FakeTargetProvider.ConnectionString);

        _ = await new DotnetTool(replay).CleanBuildAsync(project.FullName, CancellationToken.None).ConfigureAwait(false);
        var result = await new DotnetTool(replay).PublishAsync(project, target, "TestProject", CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Successfully deployed database 'TestProject'", result.StandardOutput[^1]);
        Assert.AreSequenceEqual(
            new[] { "/p:TargetDatabaseName=OtherDatabase", "/p:TargetDatabaseName=SecondDatabase", "/p:TargetDatabaseName=TestProject" },
            replay.ProcessCalls.Skip(2).Select(p => p[3]));
    }

    [TestMethod]
    public async Task ShouldStopPublishingAtFirstFailure()
    {
        var failed = new ProcessResult(1, ["error MSB3073"], ["stderr"]);
        var replay = new ReplayScenario(new Scenario()
        {
            Processes = [new RecordedProcess("publish", "OtherProject.csproj", failed)],
        });
        var project = SqlProject.Load(Path.Combine(TestPaths.ProjectReferencesTests, "Test"));

        var result = await new DotnetTool(replay).PublishAsync(project, new RunTarget(FakeTargetProvider.ConnectionString), "TestProject", CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual(1, result.ExitCode);
        Assert.AreSequenceEqual(new[] { "error MSB3073" }, result.StandardOutput);
        Assert.AreSequenceEqual(new[] { "stderr" }, result.StandardError);
        Assert.HasCount(1, replay.ProcessCalls);
    }
}