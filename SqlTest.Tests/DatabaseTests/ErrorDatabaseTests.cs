namespace SqlTest.DatabaseTests;

[TestClass]
public class ErrorDatabaseTests : BaseDatabaseTests
{
    public static IEnumerable<object[]> Images => BaseImages;

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllError(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Error; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "2");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Deploying database failed.");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%error MSB3073%");
    }
}