namespace SqlTest.UnitTests;

[TestClass]
public class StopwatchLogTests
{
    private readonly FakeConsole console = new FakeConsole();

    private readonly ManualTimeProvider time = new ManualTimeProvider();

    [TestMethod]
    public void ShouldRunBelowSecond()
    {
        var log = new StopwatchLog(this.console, this.time).Start("Something...", debug: false);
        this.time.Advance(TimeSpan.FromMilliseconds(42));
        log.Stop();

        Assert.AreEqual($"Something... 42 ms{Environment.NewLine}", this.console.Output);
    }

    [TestMethod]
    public void ShouldRunAboveSecond()
    {
        var log = new StopwatchLog(this.console, this.time).Start("Something...", debug: false);
        this.time.Advance(TimeSpan.FromMilliseconds(2500));
        log.Stop();

        Assert.AreEqual($"Something... 2 s{Environment.NewLine}", this.console.Output);
    }

    [TestMethod]
    public void ShouldRunDebugMode()
    {
        var log = new StopwatchLog(this.console, this.time).Start("Something...", debug: true);
        this.time.Advance(TimeSpan.FromMilliseconds(7));
        log.Stop();

        Assert.AreEqual($"Something...{Environment.NewLine}Something... 7 ms{Environment.NewLine}", this.console.Output);
    }

    [TestMethod]
    public void ShouldUseDefaultTitle()
    {
        new StopwatchLog(this.console, this.time).Start().Stop();

        Assert.EndsWith($" 0 ms{Environment.NewLine}", this.console.Output);
    }
}