namespace SqlTest;

public interface IProcessRunner
{
    public Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken cancellationToken);
}
