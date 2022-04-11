namespace SqlTest;

using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;

public class SqlTestContainer : IDisposable
{
    private MsSqlTestcontainer? testcontainer;

    public string InvokeImage(string image)
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

        return this.testcontainer.ConnectionString;
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