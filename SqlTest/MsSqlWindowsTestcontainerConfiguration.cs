namespace SqlTest;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

public class MsSqlWindowsTestcontainerConfiguration : MsSqlTestcontainerConfiguration
{
    public MsSqlWindowsTestcontainerConfiguration(string image)
        : base(image)
    {
    }

    public override IWaitForContainerOS WaitStrategy => Wait.ForWindowsContainer()
        .UntilCommandIsCompleted("sqlcmd", "-S", $"localhost,{this.DefaultPort}", "-U", this.Username, "-P", this.Password);
}