namespace SqlTest;

using System.Reflection;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args), "Value cannot be null.");
        }

        if (args.Length == 0)
        {
            var versionString = Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                .ToString();

            Console.WriteLine($"sqltest v{versionString}\n------------\nUsage: sqltest [options]");
        }
    }
}