using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckTotalActionFinish : WorldCondition
{
    [Serializable]
    public class ActionAndCount
    {
        public Action action;
        [Min(0)]
        public int count;
    }
    public List<ActionAndCount> targetActions;
    public override bool Judge()
    {
        return targetActions.Find(pair => pair.action.finishCount() < pair.count) == null;
    }
}
