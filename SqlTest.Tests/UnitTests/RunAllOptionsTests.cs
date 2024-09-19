namespace SqlTest.Tests.UnitTests;

[TestClass]
public class RunAllOptionsTests
{
    [TestMethod]
    public void ShouldHaveEmptyProperties()
    {
        var test = new RunAllOptions();

        Assert.IsTrue(string.IsNullOrEmpty(test.Image));
        Assert.IsTrue(string.IsNullOrEmpty(test.Project));
        Assert.IsTrue(string.IsNullOrEmpty(test.Collation));
        Assert.IsTrue(string.IsNullOrEmpty(test.Result));
        Assert.IsTrue(string.IsNullOrEmpty(test.CcCobertura));
        Assert.IsFalse(test.CcDisable);
        Assert.IsFalse(test.CcIncludeTsqlt);
    }
}