using System;
using UnityEngine;

[Serializable]
public abstract class RegionCondition
{
    public abstract bool Judge(Region region);

    [Serializable]
    public class ReorderableList : ReorderableListBase<RegionCondition>
    {
        public bool Judge(Region region)
        {
            return List.Find(condition => !condition.Judge(region)) == null;
        }
    }
    public abstract class WorldCondition : RegionCondition
    {
        public abstract bool Judge();

        public sealed override bool Judge(Region region)
        {
            return Judge();
        }
    }
    public class CheckRegionColonyTotalLevel : RegionCondition
    {
        public Animal animal;
        public int totalColonyLevel;
        public NumCompare.Type compareType;
        public override bool Judge(Region region)
        {
            return NumCompare.Judge(compareType, region.TotalRevealedColonyLevel(animal), totalColonyLevel);
        }
    }
    public class CheckRegionAnimalCount : RegionCondition
    {
        public enum CheckType { amount, change }
        public CheckType type;
        public Animal animal;
        public int value;
        public NumCompare.Type compareType;
        public override bool Judge(Region region)
        {
            int? record;
            switch (type)
            {
                case CheckType.amount:
                    record = region.GetSpeciesAmountInLatestRecord(animal);
                    break;
                case CheckType.change:
                    record = region.GetSpeciesChangeInLatestRecord(animal);
                    break;
                default:
                    record = null;
                    break;
            }
            if (record != null)
                Debug.Log(animal.animalName + ": " + record.Value);
            else
                Debug.Log("waiting for first record...");
            return record == null ? false : NumCompare.Judge(compareType, record.Value, value);
        }
    }
    public class CheckRegionBuildingCount : RegionCondition
    {
        public BuildingInfo building;
        public int value;
        public NumCompare.Type compareType;

        public override bool Judge(Region region)
        {
            return NumCompare.Judge(compareType, region.CountConstructedBuilding(building), value);
        }
    }
    public class CheckTotalActionFinish : WorldCondition
    {
        public Action action;
        public int value;
        public NumCompare.Type compareType;

        public override bool Judge()
        {
            return NumCompare.Judge(compareType, action.finishCount(), value);
        }
    }
    public class CheckTotalAnimalCount : WorldCondition
    {
        public enum CheckType { amount, change }
        public CheckType type;
        public Animal animal;
        public int value;
        public NumCompare.Type compareType;
        public override bool Judge()
        {
            switch (type)
            {
                case CheckType.amount:
                    return NumCompare.Judge(compareType, Stage.GetSpeciesAmount(animal), value);
                case CheckType.change:
                    return NumCompare.Judge(compareType, Stage.GetSpeciesChange(animal), value);
            }
            return false;
        }
    }
    public class CheckHabitatsRestored : RegionCondition
    {
        public Animal animal;
        public override bool Judge(Region region)
        {
            return region.GetAreas().Find(area =>
            {
                Habitat habitat = area.habitat;
                return habitat != null && habitat.animal.Equals(animal) && habitat.IsVisible && habitat.LastCheckedHabitability < 0.5f;
            }) == null;
        }
    }
}