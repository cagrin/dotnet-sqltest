namespace SqlTest;

public interface ICodeCoverage
{
    public void Start(string connectionString, string database, string[]? excludeFilter);

    public CoverageReport Stop();
}
