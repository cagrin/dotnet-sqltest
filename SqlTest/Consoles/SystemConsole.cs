namespace SqlTest;

public sealed class SystemConsole : IConsole
{
    private const int UnknownColor = -1;

    private ConsoleColor foregroundColor = (ConsoleColor)UnknownColor;

    public ConsoleColor ForegroundColor
    {
        get => this.foregroundColor;
        set
        {
            this.foregroundColor = value;
            Console.ForegroundColor = value;
        }
    }

    public void ResetColor()
    {
        this.foregroundColor = (ConsoleColor)UnknownColor;
        Console.ResetColor();
    }

    public void Write(string? value)
    {
        Console.Write(value);
    }

    public void WriteLine(object? value)
    {
        Console.WriteLine(value);
    }

    public void WriteLine(string? value)
    {
        Console.WriteLine(value);
    }
}