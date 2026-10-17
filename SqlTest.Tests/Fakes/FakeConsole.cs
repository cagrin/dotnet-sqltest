namespace SqlTest.Tests;

using System.Text;

internal sealed class FakeConsole : IConsole
{
    private readonly StringBuilder sb = new StringBuilder();

    public ConsoleColor ForegroundColor { get; set; } = (ConsoleColor)(-1);

    public string Output
    {
        get
        {
            lock (this.sb)
            {
                return this.sb.ToString();
            }
        }
    }

    public string[] Lines => this.Output.Split(Environment.NewLine).SkipLast(1).ToArray();

    public void ResetColor()
    {
        this.ForegroundColor = (ConsoleColor)(-1);
    }

    public void Write(string? value)
    {
        lock (this.sb)
        {
            _ = this.sb.Append(value);
        }
    }

    public void WriteLine(object? value)
    {
        this.WriteLine(value?.ToString());
    }

    public void WriteLine(string? value)
    {
        lock (this.sb)
        {
            _ = this.sb.AppendLine(value);
        }
    }
}