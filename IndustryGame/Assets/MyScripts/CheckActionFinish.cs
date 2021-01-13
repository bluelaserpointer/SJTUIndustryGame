using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Condition - CheckActionFinish")]
public class CheckActionFinish : Condition
{
    public List<Action> targetActions;
    public override bool judge()
    {
        bool completed = true;
        foreach (Action action in targetActions)
        {
            if (!Action.finishedActions.Contains(action))
            {
                completed = false;
                break;
            }
        }
        return completed;
    }
}
