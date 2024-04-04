namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class StopwatchLogTests
{
    [TestMethod]
    public void ShouldRunBelowMilisecond()
    {
        var mock = new MockIConsole();

        new StopwatchLog(mock.Object).Start("Something...", debug: false).Stop();

        Assert.That.IsLike(mock.Output, "Something... _ ms");
    }

    [TestMethod]
    public void ShouldRunAboveSecond()
    {
        var mock = new MockIConsole();

        var watch = new StopwatchLog(mock.Object).Start("Something...", debug: false);
        Thread.Sleep(1000);
        watch.Stop();

        Assert.That.IsLike(mock.Output, "Something... _ s");
    }

    [TestMethod]
    public void ShouldRunDebugMode()
    {
        var mock = new MockIConsole();

        new StopwatchLog(mock.Object).Start("Something...", debug: true).Stop();

        Assert.That.IsLike(mock.Output, "Something...\nSomething... _ ms");
    }
}