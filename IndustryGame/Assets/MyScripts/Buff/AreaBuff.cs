using System;
using UnityEngine;

[Serializable]
public abstract class AreaBuff
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
    [Serializable]
    public class ReorderableList : ReorderableListBase<AreaBuff>
    {
    }
    public class BuffAnimalAmount : AreaBuff
    {
        public Animal animal;
        public int change;
        public override void Applied(Area area, float power)
        {
        }

        public override void Idle(Area area, float power)
        {
            area.changeSpeciesAmount(animal, (int)(change * power));
        }

        public override void Removed(Area area, float power)
        {
        }
    }
}