namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class RunAllCommandTests
{
    [TestMethod]
    public void ShoudRunDispose()
    {
        using var stc = new RunAllCommand();
    }
}