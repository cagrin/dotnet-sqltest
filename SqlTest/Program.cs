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

        var imageOption = new Option<string>("--image");

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            imageOption,
        };

        rootCommand.SetHandler((string image) => InvokeSqlTest(image), imageOption);

        _ = rootCommand.Invoke(args);
    }

    public static void InvokeSqlTest(string image)
    {
        if (image != null)
        {
            using var stc = new SqlTestContainer();

            var cs = stc.InvokeImage(image);

            Console.WriteLine(cs);
        }
    }
}