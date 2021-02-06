using UnityEngine;

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
        return record == null ? false : NumCompare.Judge(compareType, record.Value, value);
    }
}
