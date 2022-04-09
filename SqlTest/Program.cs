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
            var ea = Assembly.GetEntryAssembly();
            if (ea != null)
            {
                var aiva = ea.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                if (aiva != null)
                {
                    var versionString = aiva.InformationalVersion.ToString().Split("+").First();
                    Console.WriteLine($"sqltest v{versionString}\n------------\nUsage: sqltest [options]");
                }
            }
        }
    }
}