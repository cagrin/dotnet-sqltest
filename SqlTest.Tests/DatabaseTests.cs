namespace SqlTest.Tests;

public class DatabaseTests
{
    public string Image { get; set; } =
#if DEBUG
        "cagrin/azure-sql-edge-arm64";
#else
        "mcr.microsoft.com/azure-sql-edge";
#endif
}