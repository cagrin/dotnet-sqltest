namespace SqlTest.Tests;

internal sealed class FakeTargetProvider : ITargetProvider
{
    public const string ConnectionString = "Server=localhost,1433;User ID=sa;Password=Secret;Initial Catalog=master";

    private readonly Func<CancellationToken, Task<RunTarget>> start;

    public FakeTargetProvider(Func<CancellationToken, Task<RunTarget>>? start = null)
    {
        this.start = start ?? (_ => Task.FromResult(new RunTarget(ConnectionString)));
    }

    public bool IsDisposed { get; private set; }

    public Task<RunTarget> StartAsync(CancellationToken cancellationToken)
    {
        return this.start(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        this.IsDisposed = true;
        return ValueTask.CompletedTask;
    }
}
