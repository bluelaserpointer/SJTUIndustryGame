using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public abstract void applied();
    public abstract void idle();
    public abstract void removed();
}