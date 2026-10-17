namespace SqlTest;

public sealed class StopwatchLog
{
    private readonly IConsole console;

    private readonly TimeProvider timeProvider;

    private long startTimestamp;

    private string? title;

    private bool debug;

    public StopwatchLog(IConsole console, TimeProvider timeProvider)
    {
        this.console = console;
        this.timeProvider = timeProvider;
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

        this.startTimestamp = this.timeProvider.GetTimestamp();

        return this;
    }

    public void Stop()
    {
        var elapsed = this.timeProvider.GetElapsedTime(this.startTimestamp);

        string message = elapsed.TotalSeconds < 1.0 ?
            $" {(int)elapsed.TotalMilliseconds} ms" :
            $" {(int)elapsed.TotalSeconds} s";

        if (this.debug)
        {
            message = this.title + message;
        }

        this.console.WriteLine(message);
    }
}