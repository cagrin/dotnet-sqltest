namespace SqlTest;

using System.Xml.Linq;

public sealed class SqlProject
{
    private SqlProject(string fullName, IReadOnlyList<SqlProjectReference> references)
    {
        this.FullName = fullName;
        this.References = references;
    }

    public string FullName { get; }

    public string DatabaseName => Path.GetFileNameWithoutExtension(this.FullName);

    // Only references deployed to another database; same-database references are part of the dacpac.
    public IReadOnlyList<SqlProjectReference> References { get; }

    public static SqlProject Load(string path)
    {
        string fullName = ResolveProjectFile(path);

        var root = XDocument.Load(fullName).Root!;

        return new SqlProject(fullName, GetReferences(root, fullName));
    }

    private static string ResolveProjectFile(string path)
    {
        if (Directory.Exists(path))
        {
            string? projectFile = Directory.GetFiles(path, "*.csproj").Order(StringComparer.Ordinal).FirstOrDefault();

            return Path.GetFullPath(projectFile ?? throw new SqlTestException($"No .csproj file found in '{path}'."));
        }

        if (!File.Exists(path))
        {
            throw new SqlTestException($"Database project '{path}' does not exist.");
        }

        return Path.GetFullPath(path);
    }

    private static List<SqlProjectReference> GetReferences(XElement root, string fullName)
    {
        string directory = Path.GetDirectoryName(fullName)!;

        List<SqlProjectReference> result = [];

        foreach (var reference in Elements(root, "ItemGroup").SelectMany(p => Elements(p, "ProjectReference")))
        {
            string? database = (string?)reference.Attribute("DatabaseVariableLiteralValue")
                ?? GetSqlCmdVariableDefault(root, (string?)reference.Attribute("DatabaseSqlCmdVariable"), fullName);

            if (database is null)
            {
                continue;
            }

            string include = (string?)reference.Attribute("Include")
                ?? throw new SqlTestException($"ProjectReference without Include attribute in '{fullName}'.");

            result.Add(new SqlProjectReference(Path.GetFullPath(Path.Combine(directory, include)), database));
        }

        return result;
    }

    private static string? GetSqlCmdVariableDefault(XElement root, string? variable, string fullName)
    {
        if (variable is null)
        {
            return null;
        }

        var defaultValue = Elements(root, "ItemGroup")
            .SelectMany(p => Elements(p, "SqlCmdVariable"))
            .Where(p => (string?)p.Attribute("Include") == variable)
            .SelectMany(p => Elements(p, "DefaultValue"))
            .FirstOrDefault();

        return defaultValue?.Value ?? throw new SqlTestException($"SqlCmdVariable '{variable}' has no DefaultValue in '{fullName}'.");
    }

    // Matches by local name, so both SDK-style and namespaced MSBuild projects are supported.
    private static IEnumerable<XElement> Elements(XElement parent, string localName)
    {
        return parent.Elements().Where(p => p.Name.LocalName == localName);
    }
}

public sealed record SqlProjectReference(string ProjectPath, string Database);
