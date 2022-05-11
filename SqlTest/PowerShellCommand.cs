namespace SqlTest;

using System.Management.Automation;
using System.Management.Automation.Runspaces;

public static class PowerShellCommand
{
    public static PSDataCollection<PSObject> Invoke(string script)
    {
        return InvokeAsync(script).Result;
    }

    public static async Task<PSDataCollection<PSObject>> InvokeAsync(string script)
    {
        var runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());

        runspace.Open();

        using var ps = PowerShell.Create();
        _ = ps.AddScript(script);
        ps.Runspace = runspace;

        var results = await ps.InvokeAsync().ConfigureAwait(false);

        foreach (var error in ps.Streams.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        runspace.Close();

        return results;
    }
}