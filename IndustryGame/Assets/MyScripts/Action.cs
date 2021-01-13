using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Action : ScriptableObject
{
    public static LinkedList<Action> finishedActions = new LinkedList<Action>();

    // Start is called before the first frame update
    public string actionName;
    [TextArea]
    public string description;
    [Min(0)]
    public int timeCost, moneyCost;

    [Serializable]
    public struct AbilityRequirement
    {
        public Ability ability;
        [Min(1)]
        public int level;
    }
    public List<AbilityRequirement> requiredAbilities;
    [Serializable]
    public class ActionFinishEvent : UnityEvent { }
    public ActionFinishEvent finishEvent;
    private Area area;
    private int progressedTime = -1;
    public void finishAction()
    {
        progressedTime = timeCost;
        finishEvent.Invoke();
        finishedActions.AddFirst(this);
        effect();
    }
    public abstract void effect();
    public bool isFinished()
    {
        return progressedTime == timeCost;
    }
    public void startAction()
    {
        progressedTime = 0;
    }
    public void cancelAction()
    {
        progressedTime = -1;
    }
    public bool isProgressing()
    {
        return progressedTime != -1 && progressedTime < timeCost;
    }
    public Area getArea()
    {
        return area;
    }
    public int getProgressedTime()
    {
        return progressedTime;
    }
    public double getProgressionRate()
    {
        return progressedTime == -1 ? 0.0 : (double)progressedTime / (double)timeCost;
    }

    void idle()
    {
        if(isProgressing())
        {
            ++progressedTime;
            if(progressedTime >= timeCost)
            {
                finishAction();
            }
        }
    }
}
