namespace SqlTest;

using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.SqlEdge;

public class SqlEdgeTestcontainer : ITestcontainer
{
    private SqlEdgeContainer? testcontainer;

    public SqlEdgeTestcontainer()
    {
    }

    public async Task<TestcontainerTarget> StartAsync(string image, string collation)
    {
        var container = new SqlEdgeBuilder()
            .WithImage(image)
            .WithEnvironment("MSSQL_COLLATION", TestcontainerFactory.WithCollation(collation))
            .WithLogger(NullLogger.Instance)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        this.testcontainer = container;

        return new TestcontainerTarget()
        {
            TargetPassword = SqlEdgeBuilder.DefaultPassword,
            TargetPort = container.GetMappedPublicPort(SqlEdgeBuilder.SqlEdgePort),
            TargetConnectionString = container.GetConnectionString(),
        };
    }

    public void Dispose()
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }
}