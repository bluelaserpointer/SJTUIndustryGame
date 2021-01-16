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
    [SerializeField]
    private ActionFinishEvent finishEvent;
    private Area area;
    public void finishAction()
    {
        finishEvent.Invoke();
        finishedActions.AddFirst(this);
        effect();
    }
    public void effect()
    {
        effect(null);
    }
    public abstract void effect(Area area);
    public abstract bool requireArea();
    public Area getArea()
    {
        return area;
    }
}
