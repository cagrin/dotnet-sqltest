namespace SqlTest;

using System.Diagnostics;

public sealed class ProcessRunner : IProcessRunner
{
    private readonly IConsole? echo;

    public ProcessRunner(IConsole? echo = null)
    {
        this.echo = echo;
    }

    public async Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo(fileName)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        startInfo.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en-US";

        List<string> output = [];
        List<string> error = [];

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (sender, e) => this.Collect(output, e.Data);
        process.ErrorDataReceived += (sender, e) => this.Collect(error, e.Data);

        _ = process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        return new ProcessResult(process.ExitCode, output, error);
    }

    private void Collect(List<string> lines, string? line)
    {
        if (line is null)
        {
            return;
        }

        lock (lines)
        {
            lines.Add(line);
        }

        this.echo?.WriteLine(line);
    }
}

public sealed record ProcessResult(int ExitCode, IReadOnlyList<string> StandardOutput, IReadOnlyList<string> StandardError);
