namespace SqlTest.Tests.UnitTests;

[TestClass]
public class SystemConsoleTests
{
    [TestMethod]
    public void ShouldHaveForegroundColor()
    {
        var console = SystemConsole.This;

        console.ForegroundColor = ConsoleColor.Black;

        Assert.AreEqual(ConsoleColor.Black, console.ForegroundColor);
    }

    [TestMethod]
    public void ShouldHaveResetColor()
    {
        var console = SystemConsole.This;

        console.ForegroundColor = ConsoleColor.Black;
        console.ResetColor();

        Assert.AreEqual(-1, (int)console.ForegroundColor);
    }

    [TestMethod]
    public void ShouldHaveWrites()
    {
        var console = SystemConsole.This;

        console.Write(string.Empty);
        console.WriteLine(string.Empty);
        console.WriteLine(new { });
    }

    [TestMethod]
    public void ShouldThrowsExceptionIfInvokeWithNullScript()
    {
        Should.Throw<ArgumentNullException>(() => SystemConsole.Invoke(null!));
    }
}