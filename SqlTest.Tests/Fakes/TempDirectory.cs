namespace SqlTest.Tests;

internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        this.Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"sqltest-{Guid.NewGuid():N}");
        _ = Directory.CreateDirectory(this.Path);
    }

    public string Path { get; }

    public string File(string name)
    {
        return System.IO.Path.Combine(this.Path, name);
    }

    public void Dispose()
    {
        Directory.Delete(this.Path, recursive: true);
    }
}
