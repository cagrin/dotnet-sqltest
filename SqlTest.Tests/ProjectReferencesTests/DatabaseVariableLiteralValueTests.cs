namespace SqlTest.ProjectReferencesTests;

using SqlTest.DatabaseTests;

[TestClass]
public class DatabaseVariableLiteralValueTests
{
    public static IEnumerable<object[]> Images => BaseDatabaseTests.Images;

    public string Folder { get; init; } = "../../../../ProjectReferences.Tests";

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunMainOtherTest(string image)
    {
        var results = SystemConsole.Invoke("pwsh", $"-Command dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Test; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: 0% (0/0), Duration: %");
    }
}