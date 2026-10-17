namespace SqlTest;

using Microsoft.Data.SqlClient;

public sealed class RunTarget
{
    public RunTarget(string targetConnectionString)
    {
        var builder = new SqlConnectionStringBuilder(targetConnectionString);

        if (string.IsNullOrWhiteSpace(builder.UserID) && string.IsNullOrWhiteSpace(builder.Password))
        {
            builder.IntegratedSecurity = true;
        }

        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            throw new ArgumentException("Target connection string must include Data Source (Server).", nameof(targetConnectionString));
        }

        builder.TrustServerCertificate = true;

        string dataSource = builder.DataSource;
        string server = dataSource;
        ushort port = 0;

        int separator = dataSource.LastIndexOf(',');
        if (separator > 0)
        {
            string maybePort = dataSource[(separator + 1)..].Trim();
            if (ushort.TryParse(maybePort, out ushort parsedPort))
            {
                port = parsedPort;
                server = dataSource[..separator].Trim();
            }
        }

        this.TargetDatabaseName = builder.InitialCatalog;

        // The database may not exist until it is deployed; queries use three-part names instead.
        builder.InitialCatalog = string.Empty;

        this.TargetConnectionString = builder.ConnectionString;
        this.TargetServerName = server;
        this.TargetPort = port;
        this.TargetUser = builder.IntegratedSecurity ? string.Empty : builder.UserID;
        this.TargetPassword = builder.IntegratedSecurity ? string.Empty : builder.Password;
    }

    public string TargetServerName { get; }

    public ushort TargetPort { get; }

    public string TargetUser { get; }

    public string TargetPassword { get; }

    public string TargetConnectionString { get; }

    public string TargetDatabaseName { get; }
}