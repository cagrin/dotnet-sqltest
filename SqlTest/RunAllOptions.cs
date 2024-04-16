namespace SqlTest;

public class RunAllOptions
{
    public string Image { get; set; } = default!;

    public string Project { get; set; } = default!;

    public string Collation { get; set; } = default!;

    public string Result { get; set; } = default!;

    public string CcCobertura { get; set; } = default!;

    public bool CcDisable { get; set; }

    public bool CcIncludeTsqlt { get; set; }
}