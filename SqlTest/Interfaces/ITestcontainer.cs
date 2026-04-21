namespace SqlTest;

public interface ITestcontainer
{
    public Task<RunTarget> StartAsync(string? image, string? collation);

    public void Dispose();
}