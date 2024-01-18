namespace SqlTest.DatabaseTests;

public class BaseDatabaseTests
{
    public static IEnumerable<object[]> Images => new[]
    {
        // new object[] { "mcr.microsoft.com/mssql/server" },
        new object[] { "mcr.microsoft.com/azure-sql-edge" },
    };

    public string Folder { get; init; } = "../../../../Database.Tests";
}