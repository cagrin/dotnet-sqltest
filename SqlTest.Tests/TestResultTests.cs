namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class TestResultTests
{
    [TestMethod]
    public void ShouldHaveEmptyProperties()
    {
        var test = new TestResult();

        Assert.AreEqual(string.Empty, test.Name);
        Assert.AreEqual(string.Empty, test.Result);
        Assert.AreEqual(string.Empty, test.Msg);
    }
}