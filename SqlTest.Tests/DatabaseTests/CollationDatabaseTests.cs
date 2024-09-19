namespace SqlTest.DatabaseTests;

[TestClass]
public class CollationDatabaseTests : BaseDatabaseTests
{
    public static IEnumerable<object[]> Images => BaseImages;

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllCollationPassed(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Collation --collation Polish_CI_AS; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, %");
    }

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllCollationFailed(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Collation; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "1");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 1, Passed: 0, %");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%Cannot resolve the collation conflict%");
    }
}