namespace SqlTest;

using System.Diagnostics;

public class SystemConsole : IConsole
{
    private const int UnknownColor = -1;

    private ConsoleColor foregroundColor = (ConsoleColor)UnknownColor;

    public static SystemConsole This { get; } = new SystemConsole();

    public ConsoleColor ForegroundColor
    {
        get => this.foregroundColor;
        set
        {
            this.foregroundColor = value;
            Console.ForegroundColor = value;
        }
    }

    public static string[] Invoke(string script)
    {
        script = (script ?? throw new ArgumentNullException(nameof(script))).Replace("\"", "\\\"", StringComparison.InvariantCulture);

        return Invoke("pwsh", $"-Command \"{script}\"");
    }

    public static string[] Invoke(string fileName, string arguments)
    {
        var output = new List<string?>();

        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        startInfo.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en-US";

        using var process = new Process()
        {
            StartInfo = startInfo,
        };

        process.OutputDataReceived += (sender, data) =>
        {
            output.Add(data.Data);
#if DEBUG
            Console.WriteLine(data.Data);
#endif
        };

        _ = process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();

        return output.Where(x => x != null).Select(x => x!).ToArray();
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