namespace SqlTest.DockerTests;

// Runs scenarios in-process against real adapters and a real SQL Server container; set SQLTEST_RECORD_FIXTURES=1 to refresh fixtures.
public abstract class DockerTestBase
{
    public const string Image = "mcr.microsoft.com/mssql/server:2022-latest";

    private static readonly Lazy<Task> Pull = new(() => new ProcessRunner().RunAsync(ContainerCli, ["pull", Image], CancellationToken.None));

    // Testcontainers talks to the Docker API (also exposed by Podman); only the CLI name differs.
    public static string ContainerCli => Environment.GetEnvironmentVariable("SQLTEST_CONTAINER_CLI")
        ?? (IsOnPath("docker") ? "docker" : "podman");

    protected static Task EnsureImagePulledAsync()
    {
        return Pull.Value;
    }

    protected static async Task RunScenarioAsync(string name)
    {
        RecordingScenario.UseToolCulture();

        await EnsureImagePulledAsync().ConfigureAwait(false);

        var scenario = ScenarioCase.Get(name);
        var options = scenario.ToOptions();
        options.PrepareTimeout = TimeSpan.FromMinutes(5);

        var console = new FakeConsole();
        var recording = new RecordingScenario();

        await using var container = new MsSqlTestcontainer(Image, scenario.Collation);

        var sut = new RunAllCommand(options, container, new DotnetTool(recording), recording, recording, console, TimeProvider.System);

        int exitCode = await sut.InvokeAsync().ConfigureAwait(false);

        scenario.Verify(exitCode, console.Lines);

        recording.SaveIfRequested(name);
    }

    private static bool IsOnPath(string command)
    {
        string[] extensions = OperatingSystem.IsWindows() ? [".exe", ".cmd"] : [string.Empty];

        return (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(directory => extensions.Select(extension => Path.Combine(directory, command + extension)))
            .Any(File.Exists);
    }
}