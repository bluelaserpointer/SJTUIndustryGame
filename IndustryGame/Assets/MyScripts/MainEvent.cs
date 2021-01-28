using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Event")]
public class MainEvent : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;

    [Header("隐藏级别")]
    [Range(0, 5)]
    public int hideLevel;

    public Condition spawnCondition;

    [Serializable]
    public class AreaRequirement
    {
        public EnvironmentType type;
        [Min(1)]
        public int count;
    }
    [Header("出现所需要的地区环境")]
    public List<AreaRequirement> areaRequirements;
    [Header("关键物种")]
    public List<Animal> concernedAnimals;

    [Header("完成功劳奖励")]
    [Min(0)]
    public int contribution;

    [Header("出现的所有Info")]
    public List<EventInfo> includedInfos = new List<EventInfo>();
    [Header("出现的所有全局措施")]
    public List<GlobalAction> includedGlobalActions = new List<GlobalAction>();
    [Header("出现的所有地区措施")]
    public List<AreaAction> includedAreaActions = new List<AreaAction>();

    private Region region;
    private bool _isSpawned;
    private bool _isAppeared;
    private bool _isFinished;
    public void init()
    {
        if(spawnCondition == null || spawnCondition.judge())
        {
            spawn();
        }
        if (hideLevel == 0)
            reveal();
        foreach(EventInfo info in includedInfos)
        {
            info.init();
        }
    }
    public void dayIdle()
    {
        if(_isSpawned)
        {
            foreach (EventInfo info in includedInfos)
            {
                info.dayIdle();
            }
        } else if(spawnCondition.judge())
        {
            spawn();
        }
    }
    public bool isAppeared()
    {
        return _isAppeared;
    }
    public void spawn()
    {
        List<Region> regions = Stage.GetRegions().FindAll(region => CanGenrateInRegion(region));
        if(regions.Count > 0)
        {
            _isSpawned = true;
            regions[UnityEngine.Random.Range(0, regions.Count)].AddEvent(this);
        } else
        {
            InGameLog.AddLog("failed spawn: " + eventName , Color.red);
        }
    }
    public void reveal()
    {
        _isAppeared = true;
        PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(eventName, description));
    }
    public bool isFinished()
    {
        if (_isFinished)
            return true;
        bool judge = true;
        foreach (EventInfo info in includedInfos)
        {
            if(!info.isFinished())
            {
                judge = false;
                break;
            }
        }
        if(judge)
        {
            _isFinished = true;
            PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(eventName, descriptionAfterFinish));
            Stage.AddResourceValue(ResourceType.contribution, contribution);
            region.UpdateConcernedSpecies();
        }
        return judge;
    }
    public List<EventInfo> GetInfos()
    {
        return includedInfos;
    }
    public List<EventInfo> GetRevealedInfos()
    {
        return includedInfos.FindAll(info => info.isAppeared());
    }
    public List<EventInfo> GetRevealedInfosRelatedToAnimal(Animal animal)
    {
        return includedInfos.FindAll(info => info.isAppeared() && info.showInAnimalsReport.Contains(animal));
    }
    public List<EventInfo> GetRevealedInfosRelatedToEnvironment()
    {
        return includedInfos.FindAll(info => info.isAppeared() && info.showInEnvironmentReport);
    }
    public bool CanGenrateInRegion(Region region)
    {
        return areaRequirements.Find(requirement => region.CountEnvironmentType(requirement.type) < requirement.count) == null;
    }
}
