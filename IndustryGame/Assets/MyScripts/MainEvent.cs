using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件流
/// </summary>
public class MainEvent
{
    /// <summary>
    /// 种类固定信息，通常不需要从外部访问(内部信息已被<see cref="MainEvent"/>内公开)
    /// </summary>
    private readonly MainEventSO so;
    /// <summary>
    /// 所属<see cref="Region"/>
    /// </summary>
    public readonly Region region;

    /// <summary>
    /// 事件流名称
    /// </summary>
    public string name { get { return so.eventName; } }
    /// <summary>
    /// 事件流描述 - 完成前
    /// </summary>
    public string description { get { return so.description; } }
    /// <summary>
    /// 事件流描述 - 完成后
    /// </summary>
    public string descriptionAfterFinish { get { return so.descriptionAfterFinish; } }
    /// <summary>
    /// 事件流隐藏级别 (0 ~ 5)
    /// </summary>
    public int hideLevel { get { return so.hideLevel; } }
    /// <summary>
    /// 事件阶段
    /// </summary>
    public readonly List<EventStage> eventStages = new List<EventStage>();
    public List<GlobalAction> includedGlobalActions { get { return so.includedGlobalActions; } }
    public List<AreaAction> includedAreaActions { get { return so.includedAreaActions; } }
    /// <summary>
    /// 事件流关注动物（该事件流可见时调查会进行统计的动物）
    /// </summary>
    public List<Animal> concernedAnimals { get { return so.concernedAnimals; } }
    /// <summary>
    /// 事件流完成功劳奖励
    /// </summary>
    public int contribution { get { return so.contribution; } }

    private bool isAppeared;
    private bool isFinished;
    //calculate after event finish
    private int wildReservated;
    public int WildReservated { get { return wildReservated; } }
    private int mamMadeEnvReservated;
    public int MamMadeEnvReservated { get { return mamMadeEnvReservated; } }
    private int totalReward;
    public int TotalReward { get { return totalReward; } }
    public MainEvent(MainEventSO so, Region region)
    {
        this.so = so;
        this.region = region;
        region.AddEvent(this);
        //generate eventStages
        foreach (EventStageSO eventStageSO in so.eventStages)
        {
            eventStages.Add(new EventStage(eventStageSO, this));
        }
        //generate initial animals
        foreach (Area area in region.GetAreas())
        {
            foreach (Animal animal in concernedAnimals)
            {
                area.changeSpeciesAmount(animal, UnityEngine.Random.Range(100, 200));
            }
        }
    }
    public void DayIdle()
    {
        if (!isFinished)
        {
            if (isAppeared)
            {
                //judge finish
                if (eventStages.Find(eventStage => !eventStage.IsFinished()) == null)
                {
                    Finish();
                }
            }
            else
            {
                //judge reveal
                if (hideLevel == 0)
                    Reveal();
            }
            foreach (EventStage eventStage in eventStages)
            {
                eventStage.DayIdle();
            }
        }
    }
    /// <summary>
    /// 根据事件流是否完成返回不同描述
    /// </summary>
    /// <returns><see cref="IsFinished"/> ? <see cref="descriptionAfterFinish"/> : <see cref="description"/></returns>
    public string GetDescription()
    {
        return isFinished ? descriptionAfterFinish : description;
    }
    /// <summary>
    /// 变为可见
    /// </summary>
    public void Reveal()
    {
        isAppeared = true;
        PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(name + " @ " + region.name, description));
        region.UpdateConcernedSpecies();
        FilterPanel.RefreshEventsDueToRegionSelection(region);
    }
    /// <summary>
    /// 完成事件
    /// </summary>
    public void Finish()
    {
        isFinished = true;
        //calculate rewards
        wildReservated = Stage.GetSpeciesAmount(concernedAnimals[0]);
        mamMadeEnvReservated = 0;
        totalReward = contribution + wildReservated + mamMadeEnvReservated;
        //show popUpWindow
        PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(name + " @ " + region.name, descriptionAfterFinish));
        PopUpCanvas.GenerateNewPopUpWindow(new EventClearPopUp.Data(this));
        //add rewards
        Stage.AddResourceValue(ResourceType.contribution, totalReward);
        region.UpdateConcernedSpecies();
    }
    /// <summary>
    /// 该事件流是否已完成
    /// </summary>
    /// <returns></returns>
    public bool IsFinished { get { return isFinished; } }
    /// <summary>
    /// 该事件流是否可见
    /// </summary>
    /// <returns></returns>
    public bool IsAppeared { get { return isAppeared; } }
    /// <summary>
    /// 获取可见的事件阶段
    /// </summary>
    /// <returns></returns>
    public List<EventStage> GetRevealedEventStages()
    {
        return eventStages.FindAll(eventStage => eventStage.IsAppeared());
    }
    /// <summary>
    /// 获取可见未完的事件阶段
    /// </summary>
    /// <returns></returns>
    public List<EventStage> GetRevealedUnfinishedEventStages()
    {
        return eventStages.FindAll(eventStage => eventStage.IsAppeared() && !eventStage.IsFinished());
    }
    /// <summary>
    /// 获取可见未完的事件阶段里与指定动物相关的(使用自<see cref="Stage.GetEventInfosRelatedToAnimal(Animal)"/>
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public List<EventStage> GetRevealedStagesRelatedToAnimal(Animal animal)
    {
        return eventStages.FindAll(eventStage => eventStage.IsAppeared() && eventStage.showInAnimalsReport.Contains(animal));
    }
    /// <summary>
    /// 获取可见未完的事件阶段里与环境相关的(使用自<see cref="Stage.GetEventInfosRelatedToEnvironment"/>)
    /// </summary>
    /// <returns></returns>
    public List<EventStage> GetRevealedStagesRelatedToEnvironment()
    {
        return eventStages.FindAll(eventStage => eventStage.IsAppeared() && eventStage.showInEnvironmentReport);
    }
}
