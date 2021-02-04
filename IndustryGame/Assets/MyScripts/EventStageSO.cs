using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Add ScriptableObjects/EventStage")]
public class EventStageSO : ScriptableObject
{
    public string infoName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;

    [Header("完成功劳奖励")]
    [Min(0)]
    public int contribution;

    [Header("出现在以下动物报告")]
    public List<Animal> showInAnimalsReport;
    [Header("出现在环境报告")]
    public bool showInEnvironmentReport;
    [Header("出现前需完成的事件阶段")]
    public List<EventStageSO> preFinishEventStages;
    [Header("完成条件")]
    public RegionCondition successCondition;
    [Header("完成前的环境效果")]
    public Buff buffBeforeFinish;
    [Header("完成后的环境效果")]
    public Buff buffAfterFinish;

    public bool CanAppear(MainEvent mainEvent)
    {
        foreach (EventStageSO eventStageSO in preFinishEventStages)
        {
            if (!mainEvent.eventStages.Find(eventStage => eventStage.so.Equals(eventStageSO)).IsFinished())
            {
                return false;
            }
        }
        return true;
    }
    public bool IsFinished(Region region)
    {
        return successCondition == null || successCondition.Judge(region);
    }
}