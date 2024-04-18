namespace SqlTest.Tests.UnitTests;

[TestClass]
public class RunAllOptionsTests
{
    [TestMethod]
    public void ShouldHaveEmptyProperties()
    {
        var test = new RunAllOptions();

        Assert.IsNull(test.Image);
        Assert.IsNull(test.Project);
        Assert.IsNull(test.Collation);
        Assert.IsNull(test.Result);
        Assert.IsNull(test.CcCobertura);
        Assert.IsFalse(test.CcDisable);
        Assert.IsFalse(test.CcIncludeTsqlt);
    }
}