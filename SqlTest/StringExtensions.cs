namespace SqlTest;

public static class StringExtensions
{
    public static string FirstLine(this string expression)
    {
        using var reader = new StringReader(expression);

        string first = reader.ReadLine() ?? string.Empty;

        return first == expression ? first : $"{first} [...]";
    }
}