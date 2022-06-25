namespace SqlTest;

public class SystemConsole : IConsole
{
    public static SystemConsole This { get; } = new SystemConsole();

    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    public void ResetColor()
    {
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