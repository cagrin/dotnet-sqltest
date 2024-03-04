namespace SqlTest;

using Microsoft.Data.SqlClient;

public class LocalhostTestcontainer : ITestcontainer
{
    private readonly string cs;

    public LocalhostTestcontainer(string connectionString = "Server=localhost;Integrated Security=SSPI;TrustServerCertificate=True")
    {
        this.cs = connectionString;
    }

    public async Task<TestcontainerTarget> StartAsync(string image, string collation)
    {
        using var con = new SqlConnection(this.cs);

        await con.OpenAsync().ConfigureAwait(false);

        return new TestcontainerTarget()
        {
            TargetConnectionString = this.cs,
        };
    }

    public void Dispose()
    {
        using var con = new SqlConnection(this.cs);

        con.Close();
    }
}