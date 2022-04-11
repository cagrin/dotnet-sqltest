namespace SqlTest;

using System.CommandLine;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args), "Value cannot be null.");
        }

        var rootCommand = new RootCommand() { Description = "Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects." };

        _ = rootCommand.Invoke(args);
    }
}