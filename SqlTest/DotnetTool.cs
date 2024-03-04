namespace SqlTest;

using System.Xml;

public static class DotnetTool
{
    public static string GetPublishScriptWithReferences(string project, ushort port, string database, string password)
    {
        string script = string.Empty;

        var references = TryGetProjectReferences(project);

        foreach (var reference in references)
        {
            script += GetPublishScript(project: reference.Include, port, database: reference.DatabaseVariableLiteralValue, password);

            script += "\n";
        }

        script += GetPublishScript(project, port, database, password);

        return script;
    }

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

    private static IEnumerable<ProjectReference> TryGetProjectReferences(string projectPath)
    {
        try
        {
            string projectFile = projectPath;

            if (Directory.Exists(projectPath))
            {
                projectFile = new DirectoryInfo(projectPath)
                    .GetFiles("*.csproj")
                    .First()
                    .FullName;
            }

            XmlDocument doc = new();
            doc.Load(projectFile);

            XmlNode root = doc.DocumentElement!;
            XmlNodeList references = root.SelectNodes("ItemGroup/ProjectReference")!;

            foreach (XmlNode reference in references)
            {
                XmlAttribute include = reference.Attributes!["Include"]!;
                XmlAttribute databaseVariableLiteralValue = reference.Attributes!["DatabaseVariableLiteralValue"]!;

                yield return new ProjectReference()
                {
                    Include = Path.GetFullPath(Path.Combine(projectPath, include.Value)),
                    DatabaseVariableLiteralValue = databaseVariableLiteralValue.Value,
                };
            }
        }
        finally
        {
        }
    }

    private sealed class ProjectReference
    {
        public string Include { get; init; } = default!;

        public string DatabaseVariableLiteralValue { get; init; } = default!;
    }
}