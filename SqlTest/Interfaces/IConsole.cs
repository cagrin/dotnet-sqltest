namespace SqlTest;

public interface IConsole
{
    // public ConsoleColor ForegroundColor { get; set; }
    // public void ResetColor();
    public void Write(string? value);

    public void WriteLine(string? value);
}