namespace SqlTest;

public class TestcontainerTarget
{
    public string TargetServerName { get; set; } = "localhost";

    public ushort TargetPort { get; set; }

    public string TargetDatabaseName { get; set; } = string.Empty;

    public string TargetUser { get; set; } = "sa";

    public string TargetPassword { get; set; } = string.Empty;

    public string TargetConnectionString { get; set; } = string.Empty;

    public static string GetPublishScript(string project, ushort port, string database, string password)
    {
        string script = $"dotnet publish {project}";

        script += $" /p:TargetServerName=localhost";

        script += (port > 0) ? $" /p:TargetPort={port}" : string.Empty;

        script += $" /p:TargetDatabaseName={database}";

        script += !string.IsNullOrEmpty(password) ? $" /p:TargetUser=sa /p:TargetPassword=\"{password}\"" : string.Empty;

        script += " --nologo";

        return script;
    }
}