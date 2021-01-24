using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Technology")]
public class Technology : GlobalAction
{
    [Serializable]
    public struct TypeChange
    {
        public ResourceType type;
        public float change;
    }
    public List<TypeChange> changes;
    public override void actionEffect()
    {
        foreach(TypeChange typeChange in changes)
        {
            Stage.AddResourceValue(typeChange.type, typeChange.change);
        }
    }
}
