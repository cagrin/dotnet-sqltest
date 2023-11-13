namespace SqlTest;

public interface ITestcontainer
{
    public Task<(string Password, ushort Port, string ConnectionString)> StartAsync(string image, string collation);

    public void Dispose();
}