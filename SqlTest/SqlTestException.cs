namespace SqlTest;

public sealed class SqlTestException : Exception
{
    public SqlTestException()
    {
    }

    public SqlTestException(string message)
        : base(message)
    {
    }

    public SqlTestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
