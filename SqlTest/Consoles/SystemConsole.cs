namespace SqlTest;

using System.Diagnostics;

public class SystemConsole : IConsole
{
    public static SystemConsole This { get; } = new SystemConsole();

    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    public static string[] Invoke(string fileName, string arguments)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        using var process = new Process()
        {
            StartInfo = startInfo,
        };

        _ = process.Start();

        string output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        return output.Split(new[] { '\n' }, StringSplitOptions.None).SkipLast(1).ToArray();
    }

    public void ResetColor()
    {
        _ = this.ForegroundColor;
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