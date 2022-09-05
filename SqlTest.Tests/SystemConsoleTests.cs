namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class SystemConsoleTests
{
    [TestMethod]
    public void ShoudHaveForegroundColor()
    {
        IConsole console = SystemConsole.This;

        console.ForegroundColor = ConsoleColor.Black;

        Assert.AreEqual(ConsoleColor.Black, console.ForegroundColor);
    }

    [TestMethod]
    public void ShoudHaveResetColor()
    {
        IConsole console = SystemConsole.This;

        console.ForegroundColor = ConsoleColor.Black;
        console.ResetColor();

        Assert.AreEqual(-1, (int)console.ForegroundColor);
    }

    [TestMethod]
    public void ShoudHaveWrites()
    {
        IConsole console = SystemConsole.This;

        console.Write(string.Empty);
        console.WriteLine(string.Empty);
        console.WriteLine(new { });
    }
}