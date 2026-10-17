namespace SqlTest.Tests;

internal static class TestPaths
{
    public static string DatabaseTests => FromBase("../../../../Database.Tests");

    public static string ProjectReferencesTests => FromBase("../../../../ProjectReferences.Tests");

    public static string RepositoryRoot => FromBase("../../../..");

    public static string Fixtures => FromBase("Fixtures");

    public static string FixturesSource => FromBase("../../../Fixtures");

    private static string FromBase(string relativePath)
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));
    }
}
