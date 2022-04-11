namespace SqlTest;

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public static class PowerShellExtensions
{
    public static Collection<PSObject> RunScript(string script)
    {
        var runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
        runspace.Open();

        using var ps = PowerShell.Create();
        _ = ps.AddScript(script);
        ps.Runspace = runspace;

        var results = ps.Invoke();

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        foreach (var error in ps.Streams.Error)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(error);
        }

        Console.ResetColor();
        runspace.Close();

        return results;
    }
}