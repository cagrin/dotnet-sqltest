namespace SqlTest;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

public static class MsSqlFactory
{
    private const string DefaultPassword = "A.794613";

    public static MsSqlTestcontainer CreateTestcontainer(string image, string collation, bool windowsContainer)
    {
        return windowsContainer ? CreateWindowsTestcontainer(image) : CreateUnixTestcontainer(image, collation);
    }

    private static MsSqlTestcontainer CreateUnixTestcontainer(string image, string collation)
    {
        using var config = new MsSqlTestcontainerConfiguration(image)
        {
            Password = DefaultPassword,
        };

        string name = "MSSQL_COLLATION";
        string value = string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;

        return new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(config)
            .WithEnvironment(name, value)
            .Build();
    }

    private static MsSqlTestcontainer CreateWindowsTestcontainer(string image)
    {
        using var config = new MsSqlWindowsTestcontainerConfiguration(image)
        {
            Password = DefaultPassword,
        };

        return new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(config)
            .WithExposedPort(1433)
            .Build();
    }
}