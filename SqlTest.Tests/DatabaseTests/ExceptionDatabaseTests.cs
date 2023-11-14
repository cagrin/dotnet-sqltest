namespace SqlTest.DatabaseTests;

[TestClass]
public class ExceptionDatabaseTests : BaseDatabaseTests
{
    public static new IEnumerable<object[]> Images => BaseDatabaseTests.Images;

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllException(string image)
    {
        var results = PowerShellConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Exception\n$LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "1");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 1, Passed: 0, %");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements%");
    }
}