namespace SqlTest;

using System.Diagnostics.CodeAnalysis;

public sealed class RunAllOptions
{
    public string Project { get; set; } = string.Empty;

    public string? Image { get; set; }

    public string? Collation { get; set; }

    public string? Result { get; set; }

    public string? CcCobertura { get; set; }

    public bool CcDisable { get; set; }

    public bool CcIncludeTsqlt { get; set; }

    public string? TargetConnectionString { get; set; }

    public TimeSpan PrepareTimeout { get; set; } = TimeSpan.FromMinutes(2);

    [MemberNotNullWhen(true, nameof(TargetConnectionString))]
    public bool UseExplicitTarget => !string.IsNullOrWhiteSpace(this.TargetConnectionString);
}