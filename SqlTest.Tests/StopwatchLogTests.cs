namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class StopwatchLogTests
{
    [TestMethod]
    public void ShoudRunBelowSecond()
    {
        var stopwatchLog = new StopwatchLog().Start("Something...");

        stopwatchLog.Stop();
    }
}