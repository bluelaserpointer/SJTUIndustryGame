using System.ComponentModel;

public enum SpeciesDangerType
{
    [Description("正常")]
    Normal,
    [Description("濒危")]
    Danger,
    [Description("极度濒危")]
    VeryDanger,
}
