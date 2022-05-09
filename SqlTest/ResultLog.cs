namespace SqlTest;

public static class ResultLog
{
    public static string FirstLine(this string expression)
    {
        using var reader = new StringReader(expression);

        var first = reader.ReadLine();

        if (expression != first)
        {
            first += $"  [...]";
        }

        return first;
    }
}