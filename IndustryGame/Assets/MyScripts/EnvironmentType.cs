using System.ComponentModel;

public enum EnvironmentType
{
    [Description("湖泊/河流")]
    Water,
    [Description("草原")]
    Grassland,
    [Description("森林")]
    Forest,
    [Description("原森林")]
    PrimalForest,
    [Description("沙滩")]
    Beach,
    [Description("沙漠")]
    Desert,
    [Description("山脉")]
    Mountains,
    [Description("城市")]
    City,
    [Description("田地")]
    Farmland,
    [Description("受污染")]
    Polluted,
    [Description("保护区")]
    Preserve
}

public static class EnvironmentTypeDescription
{
    public static string GetEnvironmentTypeDescription(EnvironmentType environmentType)
    {
        return EnumHelper.GetDescription(environmentType);
    }
}
