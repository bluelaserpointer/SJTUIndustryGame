using UnityEngine;

public abstract class RegionCondition : ScriptableObject
{
    public abstract bool Judge(Region region);
}