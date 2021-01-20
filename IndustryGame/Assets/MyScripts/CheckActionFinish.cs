using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Check - ActionFinish")]
public class CheckActionFinish : Condition
{
    [Serializable]
    public class ActionAndCount
    {
        public Action action;
        [Min(0)]
        public int count;
    }
    public List<ActionAndCount> targetActions;
    public override bool judge()
    {
        return targetActions.Find(pair => pair.action.finishCount() < pair.count) == null;
    }
}
