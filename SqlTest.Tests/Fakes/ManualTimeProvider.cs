namespace SqlTest.Tests;

internal sealed class ManualTimeProvider : TimeProvider
{
    private long timestamp;

    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public override long GetTimestamp()
    {
        return Interlocked.Read(ref this.timestamp);
    }

    public void Advance(TimeSpan duration)
    {
        _ = Interlocked.Add(ref this.timestamp, duration.Ticks);
    }
}
