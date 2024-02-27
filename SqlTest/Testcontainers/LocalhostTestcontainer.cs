namespace SqlTest;

using Dapper;
using Microsoft.Data.SqlClient;

public class LocalhostTestcontainer : ITestcontainer
{
    private readonly string cs;

    private readonly string database = $"sqltest_{Guid.NewGuid().ToString().Replace("-", string.Empty, StringComparison.InvariantCulture)}";

    public LocalhostTestcontainer(string connectionString = "Server=localhost;Integrated Security=SSPI;TrustServerCertificate=True")
    {
        this.cs = connectionString;
    }

    public async Task<TestcontainerTarget> StartAsync(string image, string collation)
    {
        using var con = new SqlConnection(this.cs);

        _ = await con.ExecuteAsync($"CREATE DATABASE {this.database}", commandTimeout: 0).ConfigureAwait(false);

        return new TestcontainerTarget()
        {
            TargetConnectionString = this.cs,
            TargetDatabaseName = this.database,
        };
    }

    public void Dispose()
    {
        using var con = new SqlConnection(this.cs);

        _ = con.Execute($"DROP DATABASE {this.database}", commandTimeout: 0);
    }
}