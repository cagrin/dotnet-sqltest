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

    public static string[] Invoke(string script)
    {
        script = (script ?? throw new ArgumentNullException(nameof(script))).Replace("\"", "\\\"", StringComparison.InvariantCulture);

        return Invoke("pwsh", $"-Command \"{script}\"");
    }

    public static async Task<string[]> InvokeAsync(string script)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        return Invoke(script);
    }

    public static string[] Invoke(string fileName, string arguments)
    {
        var output = new List<string?>();

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