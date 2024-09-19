namespace SqlTest.ProjectReferencesTests;

using SqlTest.DatabaseTests;

[TestClass]
public class IncludeDatabaseTests
{
    public static IEnumerable<object[]> Images => BaseDatabaseTests.BaseImages;

    public string Folder { get; init; } = "../../../../ProjectReferences.Tests";

    [TestMethod]
    [DynamicData(nameof(Images))]
    public void InvokeSqlTestRunTestMainFunction(string image)
    {
        var results = SystemConsole.Invoke($"dotnet SqlTest.dll runall --image {image} --project {this.Folder}/Test; echo $LASTEXITCODE");

        Assert.That.IsLike(results.Reverse().First().ToString(), "0");
        Assert.That.IsLike(results.Reverse().Skip(1).First().ToString(), "Failed: 0, Passed: 1, Coverage: 100% (3/3), Duration: %");
    }
}