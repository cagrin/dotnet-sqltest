namespace SqlTest.UnitTests;

[TestClass]
public class DotnetToolTests
{
    [TestMethod]
    public void ShouldGetPublishScriptWithLocal()
    {
        var sut = DotnetTool.GetPublishScript("{project}", "(local)", 0, "{database}", "{user}", "{password}");

        Assert.AreEqual("dotnet publish {project} /t:PublishDatabase /p:TargetDatabaseName=\"{database}\" /p:TargetServerName=\"(local)\" /p:TargetUser=\"{user}\" /p:TargetPassword=\"{password}\" /p:CreateNewDatabase=true", sut);
    }

    [TestMethod]
    public void ShouldGetPublishScriptWithLocalDb()
    {
        var sut = DotnetTool.GetPublishScript("{project}", "(localdb)", 0, string.Empty, string.Empty, string.Empty);

        Assert.AreEqual("dotnet publish {project} /t:PublishDatabase /p:TargetServerName=\"(localdb)\" /p:CreateNewDatabase=true", sut);
    }

    [TestMethod]
    public void ShouldGetCleanBuildScript()
    {
        var sut = DotnetTool.GetCleanBuildScript("{project}");

        Assert.AreEqual("dotnet clean {project}\ndotnet build {project}", sut);
    }
}