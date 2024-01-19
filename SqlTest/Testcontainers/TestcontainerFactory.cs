namespace SqlTest;

public static class TestcontainerFactory
{
    public static ITestcontainer Create(string image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image), "Value cannot be null.");
        }

        return image.Contains("azure-sql-edge", StringComparison.InvariantCulture) ? new SqlEdgeTestcontainer() : new MsSqlTestcontainer();
    }

    public static string WithCollation(string collation)
    {
        return string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;
    }
}