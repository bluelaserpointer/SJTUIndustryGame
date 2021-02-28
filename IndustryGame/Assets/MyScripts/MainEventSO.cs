using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件流种类固定信息
/// </summary>
[CreateAssetMenu(menuName = "Add ScriptableObjects/Event")]
public class MainEventSO : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;
    public Sprite image, imageAfterFinish;

    [Header("隐藏级别")]
    [Range(0, 5)]
    public int hideLevel;

    [Header("是否只在游戏开头生成")]
    public bool onlyGenerateAtBeginning;
    [SerializeField]
    [Range(0, 1)]
    private float generateChanceOneDay;
    [SerializeField]
    private RegionCondition generateCondition;

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

    [Header("出现的所有事件阶段")]
    public List<EventStageSO> eventStages = new List<EventStageSO>();
    [Header("出现的所有全局措施")]
    public List<GlobalAction> includedGlobalActions = new List<GlobalAction>();
    [Header("出现的所有地区措施")]
    public List<AreaAction> includedAreaActions = new List<AreaAction>();

    public MainEvent generatedInstance;

    private MainEventSO() { } //prevent instantiate from code
    /// <summary>
    /// 判断指定<see cref="Region"/>是否满足该事件流生成条件
    /// </summary>
    public bool CanGenrateInRegion(Region region)
    {
        return !region.IsOcean && (generateCondition == null || generateCondition.Judge(region)) && areaRequirements.Find(requirement => region.CountEnvironmentType(requirement.type) < requirement.count) == null;
    }
    /// <summary>
    /// 每日流程，概率满足时寻找条件满足的<see cref="Region"/>并生成该事件流
    /// </summary>
    public void DayIdle()
    {
        if (!onlyGenerateAtBeginning && generateChanceOneDay > (float)new System.Random().NextDouble())
        {
            TryGenerate();
        }
    }
    /// <summary>
    /// 尝试寻找一个条件满足的<see cref="Region"/>并生成该事件流
    /// </summary>
    public MainEvent TryGenerate()
    {
        generatedInstance = null;
        List<Region> regions = Stage.GetRegions().FindAll(region => CanGenrateInRegion(region));
        if (regions.Count > 0)
        {
            Region region = regions[UnityEngine.Random.Range(0, regions.Count)];
            generatedInstance = new MainEvent(this, region);
        }
        else
        {
            InGameLog.AddLog("failed spawn: " + eventName, Color.red);
        }
        return generatedInstance;
    }
}
