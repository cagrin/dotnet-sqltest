namespace SqlTest;

public class RunAllOptions
{
    public string Project { get; set; } = string.Empty;

    public string? Image { get; set; }

    public string? Collation { get; set; }

    public string? Result { get; set; }

    public string? CcCobertura { get; set; }

    public bool CcDisable { get; set; }

    public bool CcIncludeTsqlt { get; set; }
}