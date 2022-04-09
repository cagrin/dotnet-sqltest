namespace SqlTest.Tests;

using System.Management.Automation;
using System.Management.Automation.Runspaces;
using LikeComparison.TransactSql;

[TestClass]
public class PowerShellTests
{
    [TestMethod]
    public async Task RunScript()
    {
        using var ps = PowerShell.Create();

        _ = ps.AddScript("dotnet --version");

        var psobjs = await ps.InvokeAsync().ConfigureAwait(false);

        var psout = psobjs.First().BaseObject.ToString();

        Assert.IsTrue(psout?.Like("6.%"));
    }

    [TestMethod]
    public void RunDockerScript()
    {
        string script = @"
        docker container stop test_image
        docker container rm test_image
        ";

        RunScript(script);
    }

    private static void RunScript(string script)
    {
        var runspace = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
        runspace.Open();

        using var ps = PowerShell.Create();
        _ = ps.AddScript(script);
        ps.Runspace = runspace;

        try
        {
            var results = ps.Invoke();

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            foreach (var error in ps.Streams.Error)
            {
                Console.WriteLine(error);
            }
        }
        catch (RuntimeException ex)
        {
            Console.WriteLine(ex.Message);
        }

        runspace.Close();
    }
}