namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class RunAllCommandTests
{
    [TestMethod]
    public void ShouldRunDispose()
    {
        var mock = new MockIConsole();

        using var stc = new RunAllCommand(mock.Object);
    }
}