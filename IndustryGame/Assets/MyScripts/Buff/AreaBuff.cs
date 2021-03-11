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
        public float change;
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
    public class BuffStatChange : AreaBuff
    {
        public EnvironmentStatType statType;
        public float change;
        [Header("距离衰减比率(rate ^ dist)")]
        public float distanceAttenuation;
        [Min(1)]
        public int maxDistance;

        public override void Applied(Area area, float power)
        {
        }

        public override void Idle(Area area, float power)
        {
            area.ChangeEnvironmentFactorAffection(statType, change);
            //TODO: distance more than 1
            foreach(Area neighborArea in area.GetNeighborAreas())
            {
                neighborArea.ChangeEnvironmentFactorAffection(statType, change * distanceAttenuation);
            }
        }

        public override void Removed(Area area, float power)
        {
        }
    }
}