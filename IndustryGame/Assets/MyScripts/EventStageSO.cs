using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 事件阶段种类固定信息
/// </summary>
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

    [Header("出现前需完成的事件阶段")]
    public List<EventStageSO> preFinishEventStages;
    [Header("完成条件")]
    [Reorderable(generatablesNestClass: typeof(RegionCondition))]
    public RegionCondition.ReorderableList successCondition;
    [Header("出现在以下动物报告")]
    public List<Animal> showInAnimalsReport;
    [Header("相关的环境指标(所属事件流发生时生效其效果)")]
    public EnvironmentStatType relatedEnvironmentStat;
    
    /// <summary>
    /// 是否已完成所有前置阶段
    /// </summary>
    /// <param name="mainEvent"></param>
    /// <returns></returns>
    public bool CanAppear(EventStage eventStage)
    {
        if(!eventStage.mainEvent.IsAppeared)
        {
            return false;
        }
        foreach (EventStageSO eventStageSO in preFinishEventStages)
        {
            if (!eventStage.mainEvent.eventStages.Find(eachEventStage => eachEventStage.so.Equals(eventStageSO)).IsFinished())
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 是否已完成
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    public bool IsFinished(Region region)
    {
        return successCondition == null || successCondition.Judge(region);
    }
}