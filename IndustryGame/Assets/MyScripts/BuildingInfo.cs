using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Building")]
public class BuildingInfo : ScriptableObject
{
    public string buildingName;
    [TextArea]
    public string description;
    public GameObject model;
    public int moneyCost, timeCost;
    public bool preventPlayerConstruct;
    public List<BuildingInfo> preFinishBuildings;
    public List<AreaAction> preFinishAreaActions;
    [Header("建筑物效果")]
    public List<Buff> buffs;
    
    public void FinishConstruction()
    {
        foreach (Buff buff in buffs)
        {
            buff.applied();
        }
    }
    public void dayIdle()
    {
        foreach (Buff buff in buffs)
        {
            buff.idle();
        }
    }
    public void removed()
    {
        foreach (Buff buff in buffs)
        {
            buff.removed();
        }
    }
    public bool enabled(Area area)
    {
        return !preventPlayerConstruct && preFinishBuildings.Find(building => !area.ContainsConstructedBuildingInfo(building)) == null
            && preFinishAreaActions.Find(action => !area.ContainsFinishedAction(action)) == null;
    }
}
public class Building
{
    public readonly BuildingInfo info;
    private int constructionProgress;
    public Building(BuildingInfo info)
    {
        this.info = info;
    }
    public void DayIdle()
    {
        if(!IsConstructed())
        {
            constructionProgress += 1;
            if(IsConstructed())
            {
                FinishConstruction();
                PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow("建设完成", info.buildingName));
            }
        } else
        {
            info.dayIdle();
        }
    }
    public void FinishConstruction()
    {
        constructionProgress = info.timeCost;
        info.FinishConstruction();
    }
    public bool IsConstructed()
    {
        return constructionProgress >= info.timeCost;
    }
    public float GetConstructionRate()
    {
        return info.timeCost > 0 ? constructionProgress / info.timeCost : 1.0f;
    }
}