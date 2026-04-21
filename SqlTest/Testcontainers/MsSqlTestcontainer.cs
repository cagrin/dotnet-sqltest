namespace SqlTest;

using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public class MsSqlTestcontainer : ITestcontainer
{
    private MsSqlContainer? testcontainer;

    public MsSqlTestcontainer()
    {
    }

    public static string WithCollation(string? collation)
    {
        return string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;
    }

    public async Task<RunTarget> StartAsync(string? image, string? collation)
    {
        var container = new MsSqlBuilder(image)
            .WithEnvironment("MSSQL_COLLATION", WithCollation(collation))
            .WithLogger(NullLogger.Instance)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        this.testcontainer = container;

        return new RunTarget(container.GetConnectionString());
    }

    public void Dispose()
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }
}