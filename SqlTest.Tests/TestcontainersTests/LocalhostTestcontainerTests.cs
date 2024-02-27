namespace SqlTest.Tests;

using Testcontainers.MsSql;

[TestClass]
public class LocalhostTestcontainerTests
{
    [TestMethod]
    public async Task ShouldStartAsyncAndDispose()
    {
        var container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server")
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        var sut = new LocalhostTestcontainer(container.GetConnectionString());

        _ = await sut.StartAsync(image: null!, collation: null!).ConfigureAwait(false);

        sut.Dispose();

        await container.DisposeAsync().ConfigureAwait(false);
    }
}