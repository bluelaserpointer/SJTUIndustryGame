using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Check - Reservation")]
public class CheckReservation : Condition
{
    public enum CheckType { amount, change }
    public CheckType type;
    public Animal animal;
    public int amount;
    public override bool judge()
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
