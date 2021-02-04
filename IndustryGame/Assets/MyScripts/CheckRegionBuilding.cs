using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Check - RegionBuilding")]
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
        //TODO
        //foreach(var pair in buildingAndCountCompares)
        //{
        //}
        return false;
    }
}
