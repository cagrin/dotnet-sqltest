namespace SqlTest;

using System.Data.SqlClient;
using Dapper;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using LikeComparison.TransactSql;

public class RunAllCommand : IDisposable
{
    private readonly string database = "database_tests";

    private readonly string password = "A.794613";

    private int port;

    private string cs = string.Empty;

    private MsSqlTestcontainer? testcontainer;

    public void Invoke(string image, string project)
    {
        Console.WriteLine("Creating container...".ToString());
        this.CreateContainer(image);

        Console.WriteLine("Deploying database...".ToString());
        if (this.DeployDatabase(project))
        {
            Console.WriteLine("Running all tests....".ToString());
            this.RunTests();
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        this.testcontainer?.DisposeAsync().AsTask().Wait();
    }

    private void CreateContainer(string image)
    {
        var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration(image)
            {
                Password = this.password,
            });

        this.testcontainer = testcontainersBuilder.Build();
        this.testcontainer.StartAsync().Wait();

        this.port = this.testcontainer.Port;
        this.cs = this.testcontainer.ConnectionString;
    }

    private bool DeployDatabase(string project)
    {
        string script = $"dotnet publish {project} /p:TargetServerName=localhost /p:TargetPort={this.port} /p:TargetDatabaseName={this.database} /p:TargetUser=sa /p:TargetPassword={this.password} --nologo";

        var results = new PowerShellCommand().Invoke(script);

        if (!results.Last().ToString().Like("%Successfully deployed database%"))
        {
            string error = string.Empty;

            foreach (var result in results)
            {
                error += $"{result}\n";
            }

            error += "Deploying database failed.";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
            return false;
        }

        return true;
    }

    private void RunTests()
    {
        using var con = new SqlConnection($"{this.cs}TrustServerCertificate=True;");

        try
        {
            _ = con.Execute($"EXEC [{this.database}].tSQLt.RunAll");
        }
        catch (SqlException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
        finally
        {
            var results = con.Query<TestResult>($"SELECT Name, Result FROM [{this.database}].tSQLt.TestResult");

            int passed = results.Where(p => p.Result == "Success").Count();
            int failed = results.Where(p => p.Result == "Failure").Count();

            Console.ForegroundColor = failed > 0 ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"Failed: {failed}, Passed: {passed}.");
            Console.ResetColor();
        }
    }
}