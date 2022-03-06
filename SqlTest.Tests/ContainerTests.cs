namespace SqlTest.Tests;

using Dapper;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Microsoft.Data.SqlClient;

[TestClass]
public class ContainerTests
{
    private static MsSqlTestcontainer? testcontainer;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        var password = "A.794613";

        var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration()
            {
                Password = password,
            })
#if DEBUG
            .WithImage("cagrin/azure-sql-edge-arm64");
#else
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest");
#endif

        testcontainer = testcontainersBuilder.Build();
        testcontainer.StartAsync().Wait();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        testcontainer?.DisposeAsync().AsTask().Wait();
    }

    [TestMethod]
    public async Task TestConnection()
    {
        var actual = await ExecuteScalarAsync("SELECT CASE WHEN 'hello' LIKE 'h_llo' THEN 1 ELSE 0 END").ConfigureAwait(false);

        Assert.AreEqual(true, actual);
    }

    private static async Task<bool> ExecuteScalarAsync(string query)
    {
        string connectionString = $"{testcontainer?.ConnectionString}TrustServerCertificate=True;";

        using var connection = new SqlConnection(connectionString);

        return await connection.ExecuteScalarAsync<bool>(query).ConfigureAwait(false);
    }
}