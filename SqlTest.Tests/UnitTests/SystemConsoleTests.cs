namespace SqlTest.UnitTests;

[TestClass]
[DoNotParallelize]
public class SystemConsoleTests
{
    [TestMethod]
    public void ShouldHaveForegroundColor()
    {
        var console = new SystemConsole();

        console.ForegroundColor = ConsoleColor.Black;

        Assert.AreEqual(ConsoleColor.Black, console.ForegroundColor);

        console.ResetColor();
    }

    [TestMethod]
    public void ShouldHaveResetColor()
    {
        var console = new SystemConsole();

        console.ForegroundColor = ConsoleColor.Black;
        console.ResetColor();

        Assert.AreEqual(-1, (int)console.ForegroundColor);
    }

    [TestMethod]
    public void ShouldWriteToStandardOutput()
    {
        var original = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var console = new SystemConsole();

            console.Write("a");
            console.WriteLine("b");
            console.WriteLine((object)42);
        }
        finally
        {
            Console.SetOut(original);
        }

        Assert.AreEqual($"ab{Environment.NewLine}42{Environment.NewLine}", writer.ToString());
    }
}