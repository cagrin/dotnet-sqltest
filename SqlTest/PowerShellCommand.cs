namespace SqlTest;

using System.Management.Automation;
using System.Management.Automation.Runspaces;

public static class PowerShellCommand
{
    public static PSDataCollection<PSObject> Invoke(string script, IConsole? mockConsole = null)
    {
        return InvokeAsync(script, mockConsole).Result;
    }

    public static async Task<PSDataCollection<PSObject>> InvokeAsync(string script, IConsole? mockConsole = null)
    {
        var console = mockConsole ?? SystemConsole.This;

        var runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());

        runspace.Open();

        using var ps = PowerShell.Create();
        _ = ps.AddScript(script);
        ps.Runspace = runspace;

        var results = await ps.InvokeAsync().ConfigureAwait(false);

        foreach (var error in ps.Streams.Error)
        {
            console.ForegroundColor = ConsoleColor.Red;
            console.WriteLine(error);
            console.ResetColor();
        }

        runspace.Close();

        return results;
    }
}