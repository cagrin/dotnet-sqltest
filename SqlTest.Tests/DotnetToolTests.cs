namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class DotnetToolTests
{
    [TestMethod]
    public void ShouldGetPublishScriptWithLocalhost()
    {
        var sut = DotnetTool.GetPublishScript("{project}", 0, "{database}", null!);

        Assert.AreEqual("dotnet publish {project} /p:TargetServerName=localhost /p:TargetDatabaseName={database} --nologo", sut);
    }

    [TestMethod]
    public void ShouldGetCleanBuildScript()
    {
        var sut = DotnetTool.GetCleanBuildScript("{project}");

        Assert.AreEqual("dotnet clean {project}\ndotnet build {project}", sut);
    }
}