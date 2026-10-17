namespace SqlTest.UnitTests;

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
        Assert.IsFalse(test.UseExplicitTarget);
        Assert.AreEqual(TimeSpan.FromMinutes(2), test.PrepareTimeout);
    }

    [TestMethod]
    [DataRow(null, false)]
    [DataRow("", false)]
    [DataRow("  ", false)]
    [DataRow("Data Source=(local)", true)]
    public void ShouldUseExplicitTargetWhenConnectionStringIsSet(string? connectionString, bool expected)
    {
        var test = new RunAllOptions() { TargetConnectionString = connectionString };

        Assert.AreEqual(expected, test.UseExplicitTarget);
    }
}