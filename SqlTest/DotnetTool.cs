namespace SqlTest;

public static class DotnetTool
{
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

    public static string GetCleanBuildScript(string project)
    {
        string script = $"dotnet clean {project}";

        script += $"\ndotnet build {project}";

        return script;
    }
}