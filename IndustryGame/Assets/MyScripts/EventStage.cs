using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件阶段
/// </summary>
public class EventStage
{
    /// <summary>
    /// 种类固定信息，通常不需要从外部访问(内部信息已被<see cref="EventStage"/>内公开)
    /// </summary>
    public readonly EventStageSO so;
    /// <summary>
    /// 所属事件流
    /// </summary>
    public readonly MainEvent mainEvent;
    /// <summary>
    /// 事件阶段名称
    /// </summary>
    public string name { get { return so.infoName; } }
    /// <summary>
    /// 事件阶段描述 - 完成前
    /// </summary>
    public string description { get { return so.description; } }
    /// <summary>
    /// 事件阶段描述 - 完成后
    /// </summary>
    public string descriptionAfterFinish { get { return so.descriptionAfterFinish; } }
    /// <summary>
    /// 事件阶段完成功劳奖励
    /// </summary>
    public int contribution { get { return so.contribution; } }
    /// <summary>
    /// 在以下动物报告内显示<see cref="description"/>，仅该事件阶段已发现且未解决时
    /// </summary>
    public List<Animal> showInAnimalsReport { get { return so.showInAnimalsReport; } }
    /// <summary>
    /// 在以下环境报告内显示<see cref="description"/>，仅该事件阶段已发现且未解决时
    /// </summary>
    public bool showInEnvironmentReport { get { return so.relatedEnvironmentStat != null; } }
    /// <summary>
    /// 相关的环境指标(问题)
    /// </summary>
    public EnvironmentStatType relatedEnvironmentStat {  get { return so.relatedEnvironmentStat; } }
    /// <summary>
    /// 该事件阶段被发现前需要完成的阶段
    /// </summary>
    public List<EventStageSO> preFinishInfos { get { return so.preFinishEventStages; } }

    private bool _isAppeared;
    private bool _isFinished;

    public EventStage(EventStageSO so, MainEvent mainEvent)
    {
        this.so = so;
        this.mainEvent = mainEvent;
        //generate environment stat (problems);
        if (relatedEnvironmentStat != null)
        {
            int generateLimit = Random.Range(10, 15);
            List<Area> areas = mainEvent.region.GetAreas();
            for (int i = 0; i < generateLimit; ++i)
            {
                areas[Random.Range(0, areas.Count)].AddEnviromentStat(relatedEnvironmentStat, Random.Range(0.5f, 1.5f));
            }
        }
    }
    /// <summary>
    /// 完成该事件阶段
    /// </summary>
    public void finish()
    {
        _isAppeared = _isFinished = true;
    }
    /// <summary>
    /// 该事件阶段是否已完成
    /// </summary>
    /// <returns></returns>
    public bool IsFinished()
    {
        return _isFinished;
    }
    /// <summary>
    /// 该事件阶段是否已发现
    /// </summary>
    /// <returns></returns>
    public bool IsAppeared()
    {
        return _isAppeared;
    }
    /// <summary>
    /// 根据阶段是否完成返回不同描述
    /// </summary>
    /// <returns><see cref="IsFinished"/> ? <see cref="descriptionAfterFinish"/> : <see cref="description"/></returns>
    public string GetDescription()
    {
        return _isFinished ? descriptionAfterFinish : description;
    }
    /// <summary>
    /// 每日流程
    /// </summary>
    public void DayIdle()
    {
        if (_isAppeared)
        {
            if (_isFinished)
            {
            }
            else
            {
                if (so.IsFinished(mainEvent.region))
                {
                    _isFinished = true;
                    PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(name, descriptionAfterFinish));
                    Stage.AddResourceValue(ResourceType.contribution, contribution);
                }
            }
        }
        else
        {
            if (so.CanAppear(mainEvent))
            {
                _isAppeared = true;
                PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(mainEvent.name + " - " + name, description));
            }
        }
    }
}
