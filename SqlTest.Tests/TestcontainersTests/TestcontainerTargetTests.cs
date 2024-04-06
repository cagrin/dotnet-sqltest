namespace SqlTest.Tests;

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
}