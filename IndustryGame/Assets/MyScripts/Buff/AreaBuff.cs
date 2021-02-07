using UnityEngine;

public abstract class AreaBuff : ScriptableObject
{
    public abstract void Applied(Area area, float power);
    public abstract void Idle(Area area, float power);
    public abstract void Removed(Area area, float power);
    public void Applied(Area area)
    {
        Applied(area, 1.0f);
    }
    public void Idle(Area area)
    {
        Idle(area, 1.0f);
    }
    public void Removed(Area area)
    {
        Removed(area, 1.0f);
    }
}