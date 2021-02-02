using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Check - Building")]
public class CheckBuilding : Condition
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

    public override bool judge()
    {
        //TODO
        //foreach(var pair in buildingAndCountCompares)
        //{
        //}
        return false;
    }
}
