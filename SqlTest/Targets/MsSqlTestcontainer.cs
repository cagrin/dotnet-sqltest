namespace SqlTest;

using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MsSql;

public sealed class MsSqlTestcontainer : ITargetProvider
{
    private readonly string? image;

    private readonly string? collation;

    private MsSqlContainer? container;

    public MsSqlTestcontainer(string? image, string? collation)
    {
        this.image = image;
        this.collation = collation;
    }

    public static string WithCollation(string? collation)
    {
        return string.IsNullOrEmpty(collation) ? "SQL_Latin1_General_CP1_CI_AS" : collation;
    }

    public async Task<RunTarget> StartAsync(CancellationToken cancellationToken)
    {
        // Assigned before starting, so DisposeAsync also cleans up a container whose start was cancelled.
        this.container = new MsSqlBuilder(this.image)
            .WithEnvironment("MSSQL_COLLATION", WithCollation(this.collation))
            .WithLogger(NullLogger.Instance)
            .Build();

        await this.container.StartAsync(cancellationToken).ConfigureAwait(false);

        return new RunTarget(this.container.GetConnectionString());
    }

    public async ValueTask DisposeAsync()
    {
        if (this.container != null)
        {
            await this.container.DisposeAsync().ConfigureAwait(false);
        }
    }
}