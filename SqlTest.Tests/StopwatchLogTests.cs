namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class StopwatchLogTests
{
    [TestMethod]
    public void ShoudRunBelowMilisecond()
    {
        var mock = new MockIConsole();

        new StopwatchLog(mock.Object).Start("Something...").Stop();

        Assert.That.IsLike(mock.Output, "Something... _ ms");
    }
}