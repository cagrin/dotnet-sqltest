namespace SqlTest;

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public class PowerShellCommand
{
    private Runspace? runspace;

    public Collection<PSObject> Invoke(string script)
    {
        this.runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
        this.runspace.Open();

        using var ps = PowerShell.Create();
        _ = ps.AddScript(script);
        ps.Runspace = this.runspace;

        var results = ps.Invoke();

        foreach (var error in ps.Streams.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        this.runspace.Close();

        return results;
    }
}