namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class StopwatchLogTests
{
    [TestMethod]
    public void ShouldRunBelowMilisecond()
    {
        var mock = new MockIConsole();

        new StopwatchLog(mock.Object).Start("Something...").Stop();

        Assert.That.IsLike(mock.Output, "Something... _ ms");
    }

    [TestMethod]
    public void ShouldRunAboveSecond()
    {
        var mock = new MockIConsole();

        var watch = new StopwatchLog(mock.Object).Start("Something...");
        Thread.Sleep(1000);
        watch.Stop();

        Assert.That.IsLike(mock.Output, "Something... _ s");
    }
}