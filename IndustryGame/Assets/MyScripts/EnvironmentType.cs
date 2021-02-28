using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Environment")]
public class EnvironmentType : ScriptableObject
{
    public string environmentName;
    public NameTemplates usingNameTemplates;
    public Sprite icon;
    public int landformIndex;
    public int elevationMax;
    public int elevationMin;
    public int waterLevel;
    public int plantLevelMax;
    public int plantLevelMin;
    public int urbanLevelMax;
    public int urbanLevelMin;
    public int farmLevelMax;
    public int farmLevelMin;

    public static EnvironmentType[] GetAllTypes()
    {
        return Resources.LoadAll<EnvironmentType>("Environment");
    }
    public static EnvironmentType PickRandomOne()
    {
        EnvironmentType[] types = GetAllTypes();
        return types[UnityEngine.Random.Range(0, types.Length)];
    }

    public static EnvironmentType PickOne(int terrainTypeIndex)
    {
        EnvironmentType[] types = GetAllTypes();
        return types[terrainTypeIndex];
    }

    public static int[] indexMap = { 10,5,4,9,0,2,6,1,3,7,8}; 

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
