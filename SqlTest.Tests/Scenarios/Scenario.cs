namespace SqlTest.Tests;

using System.Text.Json;

// Everything the external world returned during one run of RunAllCommand; serialized as a fixture.
internal sealed class Scenario
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public List<RecordedProcess> Processes { get; init; } = [];

    public string? RunAllError { get; set; }

    public List<TestResult> TestResults { get; init; } = [];

    public string JUnitXml { get; set; } = string.Empty;

    public CoverageReport? Coverage { get; set; }

    public static Scenario Load(string name)
    {
        string json = File.ReadAllText(Path.Combine(TestPaths.Fixtures, $"{name}.json"));

        return JsonSerializer.Deserialize<Scenario>(json, JsonOptions)!;
    }

    public void Save(string directory, string name)
    {
        string json = JsonSerializer.Serialize(this, JsonOptions);

        json = ReplacePath(json, TestPaths.RepositoryRoot, "{RepositoryRoot}");
        json = ReplacePath(json, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "{UserProfile}");

        File.WriteAllText(Path.Combine(directory, $"{name}.json"), json);
    }

    private static string ReplacePath(string json, string path, string placeholder)
    {
        return json.Replace(JsonEncodedText.Encode(path.TrimEnd('\\', '/')).ToString(), placeholder, StringComparison.OrdinalIgnoreCase);
    }
}

// Arguments are reduced to the dotnet verb and project file name, so fixtures do not depend on machine paths or passwords.
internal sealed record RecordedProcess(string Command, string Project, ProcessResult Result)
{
    public static RecordedProcess From(IReadOnlyList<string> arguments, ProcessResult result)
    {
        return new RecordedProcess(arguments[0], Path.GetFileName(arguments[1]), result);
    }
}
