using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Environment")]
public class EnvironmentType : ScriptableObject
{
    public string environmentName;
    public NameTemplates usingNameTemplates;
    public Sprite icon;

    public static EnvironmentType[] GetAllTypes()
    {
        return Resources.LoadAll<EnvironmentType>("Environment");
    }
    public static EnvironmentType PickRandomOne()
    {
        EnvironmentType[] types = GetAllTypes();
        return types[UnityEngine.Random.Range(0, types.Length)];
    }
    /*[Description("湖泊/河流")]
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
    Preserve*/
}
