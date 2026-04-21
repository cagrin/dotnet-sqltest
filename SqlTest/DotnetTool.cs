namespace SqlTest;

using System.Xml;

public static class DotnetTool
{
    public static string GetPublishScriptWithReferences(string project, string serverName, ushort port, string database, string user, string password)
    {
        string script = string.Empty;

        var references = TryGetProjectReferences(project);

        foreach (var reference in references)
        {
            script += GetPublishScript(project: reference.Include, serverName, port, database: reference.Database, user, password);

            script += "\n";
        }

        script += GetPublishScript(project, serverName, port, database, user, password);

        return script;
    }

    public static string GetPublishScript(string project, string serverName, ushort port, string database, string user, string password)
    {
        string script = $"dotnet publish {project}";

        script += !string.IsNullOrEmpty(database) ? $" /p:TargetDatabaseName=\"{database}\"" : string.Empty;

        script += !string.IsNullOrEmpty(serverName) ? $" /p:TargetServerName=\"{serverName}\"" : string.Empty;

        script += (port > 0) ? $" /p:TargetPort={port}" : string.Empty;

        script += !string.IsNullOrEmpty(user) ? $" /p:TargetUser=\"{user}\"" : string.Empty;

        script += !string.IsNullOrEmpty(password) ? $" /p:TargetPassword=\"{password}\"" : string.Empty;

        script += $" /p:CreateNewDatabase=true";

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
        return Path.GetFileNameWithoutExtension(project);
    }

    private static List<ProjectReference> TryGetProjectReferences(string project)
    {
        var result = new List<ProjectReference>();

        try
        {
            XmlDocument doc = new();
            doc.Load(project);

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
                        Include = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project)!, include.Value)),
                        Database = databaseVariableLiteralValue.Value,
                    });
                }

                XmlAttribute? databaseSqlCmdVariable = reference.Attributes!["DatabaseSqlCmdVariable"];

                if (databaseSqlCmdVariable != null)
                {
                    XmlNode defaultValue = doc.SelectSingleNode($"//SqlCmdVariable[@Include='{databaseSqlCmdVariable.Value}']/DefaultValue")!;

                    result.Add(new ProjectReference()
                    {
                        Include = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project)!, include.Value)),
                        Database = defaultValue.InnerText,
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

        public string Database { get; init; } = default!;
    }
}