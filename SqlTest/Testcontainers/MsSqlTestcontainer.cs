namespace SqlTest;

using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public class MsSqlTestcontainer : ITestcontainer
{
    private IAsyncDisposable? testcontainer;

    public MsSqlTestcontainer()
    {
        TestcontainersSettings.Logger = NullLogger.Instance;
    }

    public async Task<(string Password, ushort Port, string ConnectionString)> StartAsync(string image, string collation)
    {
        string name = "MSSQL_COLLATION";
        string value = string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;

        var container = new MsSqlBuilder()
            .WithImage(image)
            .WithEnvironment(name, value)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        string password = MsSqlBuilder.DefaultPassword;
        ushort port = container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort);
        string cs = container.GetConnectionString();

        this.testcontainer = container;

        return (password, port, cs);
    }

    public void Dispose()
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }
}