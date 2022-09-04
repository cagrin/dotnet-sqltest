namespace SqlTest.DatabaseTests;

public class BaseDatabaseTests
{
    public string Image { get; set; } =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/azure-sql-edge";
#endif
}