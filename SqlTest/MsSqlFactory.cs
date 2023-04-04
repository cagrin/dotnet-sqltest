namespace SqlTest;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public static class MsSqlFactory
{
    public static MsSqlContainer CreateTestcontainer(string image, string collation, bool windowsContainer)
    {
        TestcontainersSettings.Logger = NullLogger.Instance;

        return windowsContainer ? CreateWindowsTestcontainer(image) : CreateUnixTestcontainer(image, collation);
    }

    private static MsSqlContainer CreateUnixTestcontainer(string image, string collation)
    {
        string name = "MSSQL_COLLATION";
        string value = string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;

        return new MsSqlBuilder()
            .WithImage(image)
            .WithEnvironment(name, value)
            .Build();
    }

    private static MsSqlContainer CreateWindowsTestcontainer(string image)
    {
        return new MsSqlBuilder()
            .WithImage(image)
            .WithWaitStrategy(Wait.ForWindowsContainer().UntilMessageIsLogged("Started SQL Server."))
            .Build();
    }
}