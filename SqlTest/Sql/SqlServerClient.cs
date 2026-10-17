namespace SqlTest;

using Dapper;
using Microsoft.Data.SqlClient;

public sealed class SqlServerClient : ISqlServerClient
{
    public static string QuoteName(string name)
    {
        return $"[{name.Replace("]", "]]", StringComparison.Ordinal)}]";
    }

    public async Task PingAsync(string connectionString, CancellationToken cancellationToken)
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string?> RunAllTestsAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        using var con = new SqlConnection(connectionString);

        try
        {
            _ = await con.ExecuteAsync(new CommandDefinition($"EXEC {QuoteName(database)}.tSQLt.RunAll", commandTimeout: 0, cancellationToken: cancellationToken)).ConfigureAwait(false);
            return null;
        }
        catch (SqlException ex)
        {
            return ex.Message;
        }
    }

    public async Task<IReadOnlyList<TestResult>> GetTestResultsAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        using var con = new SqlConnection(connectionString);

        var results = await con.QueryAsync<TestResult>(new CommandDefinition($"SELECT Name, Result, Msg FROM {QuoteName(database)}.tSQLt.TestResult", cancellationToken: cancellationToken)).ConfigureAwait(false);

        return results.AsList();
    }

    public async Task<string> GetJUnitXmlAsync(string connectionString, string database, CancellationToken cancellationToken)
    {
        using var con = new SqlConnection(connectionString);

        return await con.QueryFirstAsync<string>(new CommandDefinition($"{QuoteName(database)}.tSQLt.XmlResultFormatter", commandType: System.Data.CommandType.StoredProcedure, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }
}
