using System;
using System.Collections.Generic;
using UnityEngine;

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
