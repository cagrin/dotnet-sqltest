namespace SqlTest;

using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public class MsSqlTestcontainer : ITestcontainer
{
    private MsSqlContainer? testcontainer;

    public MsSqlTestcontainer()
    {
        TestcontainersSettings.Logger = NullLogger.Instance;
    }

    public async Task<TestcontainerTarget> StartAsync(string image, string collation)
    {
        var container = new MsSqlBuilder()
            .WithImage(image)
            .WithEnvironment("MSSQL_COLLATION", TestcontainerFactory.WithCollation(collation))
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        this.testcontainer = container;

        return new TestcontainerTarget()
        {
            TargetPassword = MsSqlBuilder.DefaultPassword,
            TargetPort = container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort),
            TargetConnectionString = container.GetConnectionString(),
            TargetDatabaseName = "database_tests",
        };
    }

    public void Dispose()
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }
}