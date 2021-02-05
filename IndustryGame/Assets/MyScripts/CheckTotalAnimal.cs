using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTotalAnimal : WorldCondition
{
    public enum CheckType { amount, change }
    public CheckType type;
    public Animal animal;
    public int amount;
    public override bool Judge()
    {
        switch(type)
        {
            case CheckType.amount:
                return Stage.GetSpeciesAmount(animal) >= amount;
            case CheckType.change:
                return Stage.getSpeciesChange(animal) >= amount;
        }
        return false;
    }
}
