namespace SqlTest;

using SQLCover;

public sealed class SqlCoverCodeCoverage : ICodeCoverage
{
    private CodeCoverage? coverage;

    private string database = string.Empty;

    public void Start(string connectionString, string database, string[]? excludeFilter)
    {
        this.database = database;
        this.coverage = new CodeCoverage(connectionString, database, excludeFilter);
        _ = this.coverage.Start();
    }

    public CoverageReport Stop()
    {
        var result = (this.coverage ?? throw new InvalidOperationException("Code coverage has not been started.")).Stop();

        var uncovered = result.Batches
            .SelectMany(batch => batch.Statements
                .Where(statement => statement.HitCount == 0)
                .Select(statement => new UncoveredStatement(batch.ObjectName, statement.Text)))
            .ToArray();

        return new CoverageReport(result.StatementCount, result.CoveredStatementCount, uncovered, result.Cobertura(this.database));
    }
}

public sealed record CoverageReport(long StatementCount, long CoveredStatementCount, IReadOnlyList<UncoveredStatement> Uncovered, string CoberturaXml)
{
    public long Percent => this.StatementCount == 0 ? 0 : (long)Math.Round((double)this.CoveredStatementCount / this.StatementCount * 100.0);
}

public sealed record UncoveredStatement(string ObjectName, string Text);
