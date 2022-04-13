namespace SqlTest;

using System.Collections.ObjectModel;
using System.Management.Automation;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;

public class RunAllCommand : IDisposable
{
    private MsSqlTestcontainer? testcontainer;

    public Collection<PSObject> Invoke(string image, string project)
    {
        var password = "A.794613";

        var testcontainersBuilder = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration()
            {
                Password = password,
            })
            .WithImage(image);

        this.testcontainer = testcontainersBuilder.Build();
        this.testcontainer.StartAsync().Wait();

        int port = this.testcontainer.Port;
        password = this.testcontainer.Password;

        string script = $"dotnet publish {project} /p:TargetServerName=localhost /p:TargetPort={port} /p:TargetDatabaseName=Database.Tests /p:TargetUser=sa /p:TargetPassword={password}";

        return PowerShellCommand.Invoke(script);
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
}