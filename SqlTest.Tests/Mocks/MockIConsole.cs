namespace SqlTest.Tests;

using System.Text;

public class MockIConsole : Mock<IConsole>
{
    private readonly StringBuilder sb = new StringBuilder();

    public MockIConsole()
    {
        _ = this.Setup(f => f.Write(It.IsAny<string?>()))
                .Callback((string? s) => this.sb.Append(s));

        _ = this.Setup(f => f.WriteLine(It.IsAny<object?>()))
                .Callback((object? o) => this.sb.AppendLine(o?.ToString()));

        _ = this.Setup(f => f.WriteLine(It.IsAny<string?>()))
                .Callback((string? s) => this.sb.AppendLine(s));
    }

    public string Output => this.sb.ToString();
}