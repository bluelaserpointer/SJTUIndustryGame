using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReorderableConditionList
{
    [SerializeReference]
    public List<RegionCondition> List;

    public bool Judge(Region region)
    {
        return List.Find(condition => !condition.Judge(region)) == null;
    }
}