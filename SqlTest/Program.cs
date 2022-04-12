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
        var projectOption = new Option<string>("--project");

        var rootCommand = new RootCommand("Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects.")
        {
            imageOption,
            projectOption,
        };

        rootCommand.SetHandler((string image, string project) => InvokeSqlTest(image, project), imageOption, projectOption);

        _ = rootCommand.Invoke(args);
    }

    public static void InvokeSqlTest(string image, string project)
    {
        if (image != null && project != null)
        {
            using var stc = new SqlTestContainer();

            var cs = stc.InvokeProject(image, project);

            Console.WriteLine(cs);
        }
    }
}