using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcesLoader
{
    public static BuildingInfo[] GetAllBuildingTypes()
    {
        return Resources.LoadAll<BuildingInfo>("Building");
    }
    public static EnvironmentStatType[] GetAllEnvironmentStatTypes()
    {
        return Resources.LoadAll<EnvironmentStatType>("EnvironmentStat");
    }
}
