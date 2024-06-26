namespace SqlTest.DatabaseTests;

[TestClass]
public class BaseDatabaseTests
{
    public static IEnumerable<object[]> Images => new[]
    {
        new object[] { "mcr.microsoft.com/mssql/server" },
        new object[] { "mcr.microsoft.com/azure-sql-edge" },
    };

    public string Folder { get; init; } = "../../../../Database.Tests";

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeDockerPullPassed(string image)
    {
        var results = SystemConsole.Invoke($"docker pull {image}; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), $"{image}:latest");
    }
}