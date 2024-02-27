namespace SqlTest;

public static class TestcontainerFactory
{
    public static ITestcontainer Create(string image)
    {
        if (string.IsNullOrEmpty(image))
        {
            return new LocalhostTestcontainer();
        }
        else if (image.Contains("azure-sql-edge", StringComparison.InvariantCulture))
        {
            return new SqlEdgeTestcontainer();
        }
        else
        {
            return new MsSqlTestcontainer();
        }
    }

    public static string WithCollation(string collation)
    {
        return string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;
    }
}