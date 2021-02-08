using System;
using UnityEngine;

[Serializable]
public abstract class RegionCondition
{
    public abstract bool Judge(Region region);
}