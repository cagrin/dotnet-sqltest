namespace SqlTest;

public interface ISqlServerClient
{
    public Task PingAsync(string connectionString, CancellationToken cancellationToken);

    // Returns the error raised by tSQLt.RunAll, or null when it completed without error.
    public Task<string?> RunAllTestsAsync(string connectionString, string database, CancellationToken cancellationToken);

    public Task<IReadOnlyList<TestResult>> GetTestResultsAsync(string connectionString, string database, CancellationToken cancellationToken);

    public Task<string> GetJUnitXmlAsync(string connectionString, string database, CancellationToken cancellationToken);
}
