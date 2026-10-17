namespace SqlTest.IntegrationTests;

using Microsoft.Data.SqlClient;

// Runs against a closed local port, so every call fails fast without a SQL Server.
[TestClass]
[TestCategory("Integration")]
public class SqlServerClientTests
{
    private const string Unreachable = "Server=tcp:127.0.0.1,1;User ID=sa;Password=x;Connect Timeout=1;ConnectRetryCount=0;TrustServerCertificate=True";

    private readonly SqlServerClient sut = new SqlServerClient();

    [TestMethod]
    public async Task ShouldThrowOnPing()
    {
        _ = await Assert.ThrowsAsync<SqlException>(() => this.sut.PingAsync(Unreachable, CancellationToken.None)).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ShouldReturnRunAllErrorMessage()
    {
        string? error = await this.sut.RunAllTestsAsync(Unreachable, "Database.Tests", CancellationToken.None).ConfigureAwait(false);

        Assert.IsFalse(string.IsNullOrEmpty(error));
    }

    [TestMethod]
    public async Task ShouldThrowOnQueries()
    {
        _ = await Assert.ThrowsAsync<SqlException>(() => this.sut.GetTestResultsAsync(Unreachable, "Database.Tests", CancellationToken.None)).ConfigureAwait(false);
        _ = await Assert.ThrowsAsync<SqlException>(() => this.sut.GetJUnitXmlAsync(Unreachable, "Database.Tests", CancellationToken.None)).ConfigureAwait(false);
    }
}
