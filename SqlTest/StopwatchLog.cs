namespace SqlTest;

using System.Diagnostics;

public class StopwatchLog
{
    private readonly IConsole console;

    private readonly Stopwatch stopwatch = new Stopwatch();

    private string? title;

    private bool debug;

    public StopwatchLog(IConsole? mockConsole = null)
    {
        this.console = mockConsole ?? SystemConsole.This;
    }

#if DEBUG
    public StopwatchLog Start(string title = "", bool debug = true)
#else
    public StopwatchLog Start(string title = "", bool debug = false)
#endif
    {
        if (debug)
        {
            this.debug = true;
            this.title = title;

            this.console.WriteLine(title);
        }
        else
        {
            this.console.Write(title);
        }

        this.stopwatch.Start();

        return this;
    }

    public void Stop()
    {
        this.stopwatch.Stop();

        string message = this.stopwatch.Elapsed.TotalSeconds < 1.0 ?
            $" {(int)this.stopwatch.Elapsed.TotalMilliseconds} ms" :
            $" {(int)this.stopwatch.Elapsed.TotalSeconds} s";

        if (this.debug)
        {
            message = this.title + message;
        }

        this.console.WriteLine(message);
    }
}