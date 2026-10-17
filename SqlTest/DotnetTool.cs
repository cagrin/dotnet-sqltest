namespace SqlTest;

using System.Globalization;

public sealed class DotnetTool
{
    public const string FileName = "dotnet";

    private const string SpecialCharacters = "%$@';,?*";

    private readonly IProcessRunner runner;

    public DotnetTool(IProcessRunner runner)
    {
        this.runner = runner;
    }

    public static IReadOnlyList<string> GetCleanArguments(string project)
    {
        return ["clean", project];
    }

    public static IReadOnlyList<string> GetBuildArguments(string project)
    {
        return ["build", project];
    }

    public static IReadOnlyList<string> GetPublishArguments(string project, RunTarget target, string database)
    {
        List<string> arguments = ["publish", project, "/t:PublishDatabase"];

        AddProperty(arguments, "TargetDatabaseName", database);
        AddProperty(arguments, "TargetServerName", target.TargetServerName);
        AddProperty(arguments, "TargetPort", target.TargetPort > 0 ? target.TargetPort.ToString(CultureInfo.InvariantCulture) : string.Empty);
        AddProperty(arguments, "TargetUser", target.TargetUser);
        AddProperty(arguments, "TargetPassword", target.TargetPassword);
        AddProperty(arguments, "CreateNewDatabase", "true");

        return arguments;
    }

    // MSBuild treats these characters specially in /p: values (';' and ',' separate properties).
    public static string EscapeProperty(string value)
    {
        return string.Concat(value.Select(c => SpecialCharacters.Contains(c, StringComparison.Ordinal)
            ? string.Create(CultureInfo.InvariantCulture, $"%{(int)c:X2}")
            : c.ToString(CultureInfo.InvariantCulture)));
    }

    public async Task<ProcessResult> CleanBuildAsync(string project, CancellationToken cancellationToken)
    {
        _ = await this.runner.RunAsync(FileName, GetCleanArguments(project), cancellationToken).ConfigureAwait(false);

        return await this.runner.RunAsync(FileName, GetBuildArguments(project), cancellationToken).ConfigureAwait(false);
    }

    // Publishes referenced databases first and stops at the first failure.
    public async Task<ProcessResult> PublishAsync(SqlProject project, RunTarget target, string database, CancellationToken cancellationToken)
    {
        List<string> output = [];
        List<string> error = [];

        var publishes = project.References.Append(new SqlProjectReference(project.FullName, database));

        foreach (var publish in publishes)
        {
            var result = await this.runner.RunAsync(FileName, GetPublishArguments(publish.ProjectPath, target, publish.Database), cancellationToken).ConfigureAwait(false);

            output.AddRange(result.StandardOutput);
            error.AddRange(result.StandardError);

            if (result.ExitCode != 0)
            {
                return new ProcessResult(result.ExitCode, output, error);
            }
        }

        return new ProcessResult(0, output, error);
    }

    private static void AddProperty(List<string> arguments, string name, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            arguments.Add($"/p:{name}={EscapeProperty(value)}");
        }
    }
}