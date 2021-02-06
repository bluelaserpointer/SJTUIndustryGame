using UnityEngine;

public abstract class WorldCondition : RegionCondition
{
    public abstract bool Judge();

    public sealed override bool Judge(Region region)
    {
        return Judge();
    }
}
