namespace SqlTest.DatabaseTests;

[TestClass]
public class FailDatabaseTests : BaseDatabaseTests
{
    public static new IEnumerable<object[]> Images => BaseDatabaseTests.Images;

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunAllFail(string image)
    {
        var results = SystemConsole.Invoke("pwsh", $"-Command dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Fail; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "1");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 1, Passed: 0, %");
        Assert.That.IsLike(results.Reverse().Skip(2).First().ToString(), "%failed%");
    }
}