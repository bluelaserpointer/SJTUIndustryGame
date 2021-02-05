using UnityEngine;

public class CheckRegionReservationCount : RegionCondition
{
    public enum CheckType { amount, change }
    public CheckType type;
    public Animal animal;
    public int amount;
    public override bool Judge(Region region)
    {
        /*
        switch (type)
        {
            case CheckType.amount:
                return Stage.getSpeciesAmount(animal) >= amount;
            case CheckType.change:
                return Stage.getSpeciesChange(animal) >= amount;
        }*/
        return false;
    }
}
