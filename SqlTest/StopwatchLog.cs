namespace SqlTest;

using System.Diagnostics;

public class StopwatchLog
{
    private readonly IConsole console;

    private readonly Stopwatch stopwatch = new Stopwatch();

    public StopwatchLog(IConsole? mockConsole = null)
    {
        this.console = mockConsole ?? SystemConsole.This;
    }

    public StopwatchLog Start(string message = "")
    {
        this.console.Write(message);

        this.stopwatch.Start();

        return this;
    }

    public void Stop()
    {
        this.stopwatch.Stop();

        string message = this.stopwatch.Elapsed.TotalSeconds < 1.0 ?
            $" {(int)this.stopwatch.Elapsed.TotalMilliseconds} ms" :
            $" {(int)this.stopwatch.Elapsed.TotalSeconds} s";

        this.console.WriteLine(message);
    }
}