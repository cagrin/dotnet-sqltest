namespace SqlTest;

public class TestcontainerTarget
{
    public string TargetServerName { get; set; } = "localhost";

    public ushort TargetPort { get; set; }

    public string TargetUser { get; set; } = "sa";

    public string TargetPassword { get; set; } = string.Empty;

    public string TargetConnectionString { get; set; } = string.Empty;
}