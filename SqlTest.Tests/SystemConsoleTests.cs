namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class SystemConsoleTests
{
    [TestMethod]
    public void ShouldHaveForegroundColor()
    {
        IConsole console = SystemConsole.This;

        console.ForegroundColor = ConsoleColor.Black;

        Assert.AreEqual(ConsoleColor.Black, console.ForegroundColor);
    }

    [TestMethod]
    public void ShouldHaveResetColor()
    {
        IConsole console = SystemConsole.This;

        console.ForegroundColor = ConsoleColor.Black;
        console.ResetColor();

        Assert.AreEqual(-1, (int)console.ForegroundColor);
    }

    [TestMethod]
    public void ShouldHaveWrites()
    {
        IConsole console = SystemConsole.This;

        console.Write(string.Empty);
        console.WriteLine(string.Empty);
        console.WriteLine(new { });
    }
}