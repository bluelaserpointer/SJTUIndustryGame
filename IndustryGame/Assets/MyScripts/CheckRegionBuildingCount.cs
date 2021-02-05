using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckRegionBuildingCount : RegionCondition
{
    public BuildingInfo building;
    public int value;
    public NumCompare.Type compareType;

    public override bool Judge(Region region)
    {
        return NumCompare.Judge(compareType, region.CountConstructedBuilding(building), value);
    }
}
