using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Action : ScriptableObject
{
    public string actionName;
    [TextArea]
    public string description;
    [Min(0)]
    public int timeCost, moneyCost;
    [Header("可用前需完成的事件阶段")]
    public List<EventStageSO> preFinishInfos;

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

    //专家不管目前做的措施是什么，完成后固定传参自己的目前位置。如果这是全局措施则会被无视。
    public abstract void finishAction(Area area);

    public int finishCount()
    {
        return Stage.GetActionFinishCount(this);
    }
    public bool finishedOnce()
    {
        return finishCount() > 0;
    }
}