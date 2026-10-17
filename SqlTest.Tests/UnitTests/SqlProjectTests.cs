namespace SqlTest.UnitTests;

[TestClass]
public class SqlProjectTests
{
    [TestMethod]
    public void ShouldLoadProjectFromDirectory()
    {
        var sut = SqlProject.Load(Path.Combine(TestPaths.DatabaseTests, "Ok"));

        Assert.AreEqual(Path.Combine(TestPaths.DatabaseTests, "Ok", "Database.Tests.csproj"), sut.FullName);
        Assert.AreEqual("Database.Tests", sut.DatabaseName);
        Assert.IsEmpty(sut.References);
    }

    [TestMethod]
    public void ShouldLoadProjectFromFile()
    {
        var sut = SqlProject.Load(Path.Combine(TestPaths.ProjectReferencesTests, "Main", "MainProject.csproj"));

        Assert.AreEqual("MainProject", sut.DatabaseName);
        Assert.HasCount(2, sut.References);
    }

    [TestMethod]
    public void ShouldResolveReferencesByLiteralValueAndSqlCmdVariable()
    {
        var sut = SqlProject.Load(Path.Combine(TestPaths.ProjectReferencesTests, "Test"));

        Assert.AreSequenceEqual(
            new[]
            {
                new SqlProjectReference(Path.Combine(TestPaths.ProjectReferencesTests, "Other", "OtherProject.csproj"), "OtherDatabase"),
                new SqlProjectReference(Path.Combine(TestPaths.ProjectReferencesTests, "Second", "SecondProject.csproj"), "SecondDatabase"),
            },
            sut.References);
    }

    [TestMethod]
    public void ShouldSupportNamespacedProjects()
    {
        using var temp = new TempDirectory();
        string project = """
            <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <ItemGroup>
                <ProjectReference Include="Other.csproj" DatabaseVariableLiteralValue="OtherDatabase" />
              </ItemGroup>
            </Project>
            """;
        File.WriteAllText(temp.File("Legacy.csproj"), project);

        var sut = SqlProject.Load(temp.Path);

        Assert.AreEqual(new SqlProjectReference(temp.File("Other.csproj"), "OtherDatabase"), sut.References.Single());
    }

    [TestMethod]
    public void ShouldThrowWhenDirectoryHasNoProject()
    {
        using var temp = new TempDirectory();

        var ex = Assert.Throws<SqlTestException>(() => SqlProject.Load(temp.Path));

        Assert.Contains("No .csproj file found", ex.Message);
    }

    [TestMethod]
    public void ShouldThrowWhenProjectDoesNotExist()
    {
        var ex = Assert.Throws<SqlTestException>(() => SqlProject.Load("does-not-exist.csproj"));

        Assert.Contains("does not exist", ex.Message);
    }

    [TestMethod]
    public void ShouldThrowWhenSqlCmdVariableHasNoDefaultValue()
    {
        using var temp = new TempDirectory();
        string project = """
            <Project Sdk="MSBuild.Sdk.SqlProj">
              <ItemGroup>
                <ProjectReference Include="Other.csproj" DatabaseSqlCmdVariable="Missing" />
              </ItemGroup>
            </Project>
            """;
        File.WriteAllText(temp.File("Main.csproj"), project);

        var ex = Assert.Throws<SqlTestException>(() => SqlProject.Load(temp.Path));

        Assert.Contains("SqlCmdVariable 'Missing' has no DefaultValue", ex.Message);
    }

    [TestMethod]
    public void ShouldThrowWhenReferenceHasNoInclude()
    {
        using var temp = new TempDirectory();
        string project = """
            <Project Sdk="MSBuild.Sdk.SqlProj">
              <ItemGroup>
                <ProjectReference DatabaseVariableLiteralValue="OtherDatabase" />
              </ItemGroup>
            </Project>
            """;
        File.WriteAllText(temp.File("Main.csproj"), project);

        var ex = Assert.Throws<SqlTestException>(() => SqlProject.Load(temp.Path));

        Assert.Contains("without Include", ex.Message);
    }
}
