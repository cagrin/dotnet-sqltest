namespace SqlTest;

using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.SqlEdge;

public class SqlEdgeTestcontainer : ITestcontainer
{
    private SqlEdgeContainer? testcontainer;

    public SqlEdgeTestcontainer()
    {
        TestcontainersSettings.Logger = NullLogger.Instance;
    }

    public async Task<(string Password, ushort Port, string ConnectionString)> StartAsync(string image, string collation)
    {
        var container = new SqlEdgeBuilder()
            .WithImage(image)
            .WithEnvironment("MSSQL_COLLATION", TestcontainerFactory.WithCollation(collation))
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        string password = SqlEdgeBuilder.DefaultPassword;
        ushort port = container.GetMappedPublicPort(SqlEdgeBuilder.SqlEdgePort);
        string cs = container.GetConnectionString();

        this.testcontainer = container;

        return (password, port, cs);
    }

    public void Dispose()
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }
}