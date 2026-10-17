namespace SqlTest;

public sealed class ExplicitTarget : ITargetProvider
{
    private readonly string connectionString;

    private readonly ISqlServerClient client;

    public ExplicitTarget(string connectionString, ISqlServerClient client)
    {
        this.connectionString = connectionString;
        this.client = client;
    }

    public async Task<RunTarget> StartAsync(CancellationToken cancellationToken)
    {
        var target = new RunTarget(this.connectionString);

        await this.client.PingAsync(target.TargetConnectionString, cancellationToken).ConfigureAwait(false);

        return target;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
