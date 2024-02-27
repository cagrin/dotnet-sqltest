namespace SqlTest;

public interface ITestcontainer
{
    public Task<TestcontainerTarget> StartAsync(string image, string collation);

    public void Dispose();
}