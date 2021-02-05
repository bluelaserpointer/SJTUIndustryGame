using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTotalAnimalCount : WorldCondition
{
    public enum CheckType { amount, change }
    public CheckType type;
    public Animal animal;
    public int value;
    public NumCompare.Type compareType;
    public override bool Judge()
    {
        switch(type)
        {
            case CheckType.amount:
                return NumCompare.Judge(compareType, Stage.GetSpeciesAmount(animal), value);
            case CheckType.change:
                return NumCompare.Judge(compareType, Stage.GetSpeciesChange(animal), value);
        }
        return false;
    }
}
