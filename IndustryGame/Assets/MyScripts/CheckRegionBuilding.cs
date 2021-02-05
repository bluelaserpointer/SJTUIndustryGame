using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckRegionBuilding : RegionCondition
{
    public enum compareType { large, largeEqual, small, smallEqual, equal };
    [Serializable]
    public struct BuildingAndCountCompare
    {
        public BuildingInfo target;
        [Min(0)]
        public int count;
        public compareType compareType;
    }
    public List<BuildingAndCountCompare> buildingAndCountCompares;

    public override bool Judge(Region region)
    {
        foreach(var pair in buildingAndCountCompares)
        {
            int count = region.CountBuilding(pair.target);

        }
        return false;
    }
}
