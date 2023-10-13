namespace SqlTest;

using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public static class MsSqlFactory
{
    public static MsSqlContainer CreateTestcontainer(string image, string collation)
    {
        TestcontainersSettings.Logger = NullLogger.Instance;

        return CreateUnixTestcontainer(image, collation);
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
}