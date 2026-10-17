namespace SqlTest;

public interface ITargetProvider : IAsyncDisposable
{
    public Task<RunTarget> StartAsync(CancellationToken cancellationToken);
}