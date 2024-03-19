namespace SqlTest;

using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public class MsSqlTestcontainer : ITestcontainer
{
    private MsSqlContainer? testcontainer;

    public MsSqlTestcontainer()
    {
    }

    public async Task<TestcontainerTarget> StartAsync(string image, string collation)
    {
        var container = new MsSqlBuilder()
            .WithImage(image)
            .WithEnvironment("MSSQL_COLLATION", TestcontainerFactory.WithCollation(collation))
            .WithLogger(NullLogger.Instance)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        this.testcontainer = container;

        return new TestcontainerTarget()
        {
            TargetPassword = MsSqlBuilder.DefaultPassword,
            TargetPort = container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort),
            TargetConnectionString = container.GetConnectionString(),
        };
    }

    public void Dispose()
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }
}