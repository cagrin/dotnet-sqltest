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

    public static string GetProjectFullName(string project)
    {
        string projectFile = project;

        if (Directory.Exists(project))
        {
            projectFile = new DirectoryInfo(project)
                .GetFiles("*.csproj")
                .First()
                .FullName;
        }

        return projectFile;
    }

    public static string GetDatabaseName(string project)
    {
        string projectFile = GetProjectFullName(project);

        return Path.GetFileNameWithoutExtension(projectFile);
    }

    private static List<ProjectReference> TryGetProjectReferences(string project)
    {
        List<ProjectReference> result = new();

        try
        {
            string projectFile = GetProjectFullName(project);

            XmlDocument doc = new();
            doc.Load(projectFile);

            XmlNode root = doc.DocumentElement!;
            XmlNodeList references = root.SelectNodes("ItemGroup/ProjectReference")!;

            foreach (XmlNode reference in references)
            {
                XmlAttribute include = reference.Attributes!["Include"]!;
                XmlAttribute? databaseVariableLiteralValue = reference.Attributes!["DatabaseVariableLiteralValue"];

                if (databaseVariableLiteralValue != null)
                {
                    result.Add(new ProjectReference()
                    {
                        Include = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFile)!, include.Value)),
                        DatabaseVariableLiteralValue = databaseVariableLiteralValue.Value,
                    });
                }
            }
        }
        finally
        {
        }

        return result;
    }

    private sealed class ProjectReference
    {
        public string Include { get; init; } = default!;

        public string DatabaseVariableLiteralValue { get; init; } = default!;
    }
}