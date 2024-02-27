namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class TestcontainerTargetTests
{
    [TestMethod]
    public void ShouldHaveDefaults()
    {
        var sut = new TestcontainerTarget();

        Assert.AreEqual("localhost", sut.TargetServerName);
        Assert.AreEqual("sa", sut.TargetUser);
    }

    [TestMethod]
    public void ShouldGetPublishScriptWithLocalhost()
    {
        var sut = TestcontainerTarget.GetPublishScript("{project}", 0, "{database}", null!);

        Assert.AreEqual("dotnet publish {project} /p:TargetServerName=localhost /p:TargetDatabaseName={database} --nologo", sut);
    }
}