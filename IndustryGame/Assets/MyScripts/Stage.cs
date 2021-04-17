using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public enum ResourceType
{
    [Description("金钱")]
    money,
    [Description("功劳")]
    contribution,
    [Description("名声")]
    reputation,
    [Description("评价")]
    opinion,
    [Description("专家培养加成")]
    specialistTrainBoost,
    [Description("措施执行加成")]
    actionProgressBoost
}

[DisallowMultipleComponent]
public class Stage : MonoBehaviour
{
    private static Stage instance;

    private Area[] areas;
    private List<Region> regions = new List<Region>();

    public string stageName;
    [TextArea]
    public string description;
    [Header("初始事件")]
    public List<MainEventSO> definedEvents;
    public List<MainEvent> events = new List<MainEvent>();
    [Header("Money allowed for this stage")]
    public const int INITIAL_MONEY = 2000;
    [SerializeField]
    private Canvas canvasForSurroundersHUD;
    public static Canvas CanvasForSurroundersHUD => instance.canvasForSurroundersHUD;

    private HexGrid hexGrid;

    //基地资源
    private int lastDay;
    public Dictionary<ResourceType, AmountChange> resources = new Dictionary<ResourceType, AmountChange>();
    private List<Specialist> specialists = new List<Specialist>();
    private List<GlobalAction> includedGlobalActions = new List<GlobalAction>();
    private List<AreaAction> includedAreaActions = new List<AreaAction>();
    private List<BuildingInfo> includedBuildings = new List<BuildingInfo>();
    private Dictionary<Action, int> actionsFinishCount = new Dictionary<Action, int>();
    private List<Animal> concernedAnimals = new List<Animal>();

    //摄像机聚焦最高位置
    private Vector3 highestPosition;

    private Stage() { }
    void Awake()
    {
        if (instance == null)
            instance = this;
        //init NameTemplates
        NameTemplates.ResetAll();
        //init resources
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            resources.Add(resourceType, new AmountChange(0));
        //set stage money objective
        resources[ResourceType.money].AddWithoutRecording(INITIAL_MONEY);
        //load actions
        foreach (AreaAction areaAction in Resources.LoadAll<AreaAction>("Action/AreaAction"))
        {
            includedAreaActions.Add(areaAction);
        }
        foreach (GlobalAction globalAction in Resources.LoadAll<GlobalAction>("Action/GlobalAction"))
        {
            includedGlobalActions.Add(globalAction);
        }
        //load buldings
        foreach (BuildingInfo building in Resources.LoadAll<BuildingInfo>("Building"))
        {
            includedBuildings.Add(building);
        }
        // BuildingsBar.instance.RefreshList();
    }
    public static void Init(HexCell[] hexCells)
    {
        //collect all Area Components in children gameObject

        instance.areas = new Area[hexCells.Length];
        for (int i = 0; i < hexCells.Length; ++i)
        {
            instance.areas[i] = hexCells[i].GetComponentInChildren<Area>();
        }

        //generate regions
        foreach (Area area in instance.areas)
        {
            int regionId = area.GetHexCell().RegionId;

            Region region = instance.regions.Find(eachRegion => eachRegion.GetRegionId() == regionId);
            // Debug.Log("Region: " + area.region.GetRegionId() + " " + area.transform.position);
            if (region == null)
            {
                instance.regions.Add(region = new Region(regionId));
            }
            region.AddArea(area);
            area.region = region;

        }

        List<Vector3> regionHighestPositions = new List<Vector3>();
        foreach (Region region in instance.regions)
        {
            region.CalculateCenter();
            region.CalculateHighestPosition();
            regionHighestPositions.Add(region.GetHighestPosition());
        }
        List<Vector3> orderedPositions = regionHighestPositions.OrderByDescending(p => p.y).ToList();
        instance.highestPosition = orderedPositions[0];



        List<Region> xPos = instance.regions.GetRange(0, instance.regions.Count);
        List<Region> zPos = instance.regions.GetRange(0, instance.regions.Count);
        xPos.RemoveAll(r => r.GetRegionId() == -1);
        zPos.RemoveAll(r => r.GetRegionId() == -1);

        xPos = xPos.OrderBy(r => r.GetCenter().x).ToList();
        zPos = zPos.OrderBy(r => r.GetCenter().z).ToList();

        String xIds = "";
        String zIds = "";
        foreach (Region region in xPos)
            xIds += region.GetRegionId() + ": " + region.GetCenter().x + ", ";
        foreach (Region region in zPos)
            zIds += region.GetRegionId() + ": " + region.GetCenter().z + ", ";



        // Debug.Log(xIds);
        // Debug.Log(zIds);

        if (instance.regions.Count > 1)
        {
            for (int i = 0; i < xPos.Count - 1; i++)
            {
                xPos[i].SetEastRegion(xPos[i + 1]);
                xPos[i + 1].SetWestRegion(xPos[i]);
            }

            for (int i = 0; i < zPos.Count - 1; i++)
            {
                zPos[i].SetNorthRegion(zPos[i + 1]);
                zPos[i + 1].SetSouthRegion(zPos[i]);
            }
        }


        //generate initial events
        foreach (MainEventSO definedEvent in instance.definedEvents)
        {
            MainEvent mainEvent = definedEvent.TryGenerate();
            if (mainEvent != null)
                instance.events.Add(mainEvent);
        }

        //debug
        //foreach(Region region in regions) {
        //    InGameLog.AddLog("region id " + region.GetRegionId() + " area " + region.GetAreas().Count + " from " + areas.Length);
        //}
        //generate specialist employment list
        SpecialistEmployList.refresh();
        instance.hexGrid = instance.GetComponent<HexGrid>();
        instance.lastDay = Timer.GetDay();
        //union actions
        foreach (Region region in instance.regions)
        {
            if (region.MainEvent != null)
            {
                foreach (GlobalAction action in region.MainEvent.includedGlobalActions)
                {
                    if (!instance.includedGlobalActions.Contains(action))
                        instance.includedGlobalActions.Add(action);
                }
                foreach (AreaAction action in region.MainEvent.includedAreaActions)
                {
                    if (!instance.includedAreaActions.Contains(action))
                        instance.includedAreaActions.Add(action);
                }
            }
        }
    }
    void Update()
    {
        //region reservation
        foreach (Region region in regions)
        {
            if (!Timer.IsPaused())
                region.FrameIdle();
            region.UpdateNameDisplay();
        }
        //check time
        Timer.idle();
        if (lastDay != Timer.GetDay()) //day change happened
        {
            lastDay = Timer.GetDay();
            //record resource changes
            foreach (AmountChange amountChange in resources.Values)
            {
                amountChange.RecordChange();
            }
            //region dayidle
            foreach (Region region in regions)
            {
                region.DayIdle();
            }
            //specialist dayidle
            foreach (Specialist specialist in specialists)
            {
                specialist.dayIdle();
            }
            //defined events dayIdle(generate etc)
            foreach (MainEvent so in events)
            {
                so.DayIdle();
            }
        }
    }
    /// <summary>
    /// 返回<see cref="HexGrid"/>
    /// </summary>
    /// <returns></returns>
    public static HexGrid GetHexGrid()
    {
        return instance.hexGrid;
    }
    /// <summary>
    /// 获取所有地区
    /// </summary>
    /// <returns></returns>
    public static Area[] getAreas()
    {
        return instance.areas;
    }
    /// <summary>
    /// 获取所有洲
    /// </summary>
    /// <returns></returns>
    public static List<Region> GetRegions()
    {
        return instance.regions;
    }
    /// <summary>
    /// 获取所有洲的最高聚焦位置
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetHighestPosition()
    {
        return instance.highestPosition;
    }
    /// <summary>
    /// 更新濒危动物列表
    /// </summary>
    public static void UpdateConcernedSpecies()
    {
        instance.concernedAnimals.Clear();
        foreach (Region region in instance.regions)
        {
            foreach (Animal animal in region.GetConcernedSpecies())
            {
                if (!instance.concernedAnimals.Contains(animal))
                    instance.concernedAnimals.Add(animal);
            }
        }
    }
    /// <summary>
    /// 获取濒危动物列表
    /// </summary>
    /// <returns></returns>
    public static List<Animal> GetConcernedSpecies()
    {
        return instance.concernedAnimals;
    }
    /// <summary>
    /// 获取指定动物数量
    /// </summary>
    /// <param name="species"></param>
    /// <returns></returns>
    public static int GetSpeciesAmount(Animal species)
    {
        if (instance == null)
            return 0;
        int total = 0;
        foreach (Area area in instance.areas)
        {
            total += area.GetSpeciesAmount(species);
        }
        return total;
    }
    /// <summary>
    /// 获取指定动物增减数
    /// </summary>
    /// <param name="species"></param>
    /// <returns></returns>
    public static int GetSpeciesChange(Animal species)
    {
        if (instance == null)
            return 0;
        int total = 0;
        foreach (Area area in instance.areas)
        {
            total += area.GetSpeciesChange(species);
        }
        return total;
    }
    /// <summary>
    /// 获取所有已雇佣专家
    /// </summary>
    /// <returns></returns>
    public static List<Specialist> GetSpecialists()
    {
        if (instance == null)
            return new List<Specialist>();
        return instance.specialists;
    }
    /// <summary>
    /// 获取指定类型总部资源
    /// </summary>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    public static float GetResourceValue(ResourceType resourceType)
    {
        return instance.resources[resourceType].GetCurrentValue();
    }
    /// <summary>
    /// 强制增加指定类型总部资源
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void AddResourceValue(ResourceType resourceType, float value)
    {
        instance.resources[resourceType].AddWithoutRecording(value);
    }
    /// <summary>
    /// 增加指定类型总部资源<para></para>
    /// 如果结果值小于零则放弃更改并返回false
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryAddResourceValue(ResourceType resourceType, float value)
    {
        float result = instance.resources[resourceType].GetCurrentValue() + value;
        if (result < 0)
            return false;
        instance.resources[resourceType].AddWithoutRecording(value);
        return true;
    }
    /// <summary>
    /// 获取所有事件流
    /// </summary>
    /// <returns></returns>
    public static List<MainEvent> GetEvents()
    {
        return instance.events;
    }
    /// <summary>
    /// 获取所有揭开事件流
    /// </summary>
    /// <returns></returns>
    public static List<MainEvent> GetRevealedEvents()
    {
        return instance.events.FindAll(eachEvent => eachEvent.IsAppeared);
    }
    /// <summary>
    /// 获取所有揭开未完事件流
    /// </summary>
    /// <returns></returns>
    public static List<MainEvent> GetRevealedUnfinishedEvents()
    {
        return instance.events.FindAll(eachEvent => eachEvent.IsAppeared && !eachEvent.IsFinished);
    }
    /// <summary>
    /// 获取累计环境危害最大的事件
    /// </summary>
    /// <returns></returns>
    public static MainEvent GetMostHarmfulRevealedUnfinishedEvent()
    {
        MainEvent mostHarmful = null;
        float value = 0;
        foreach (MainEvent eachEvent in instance.events)
        {
            if (eachEvent.IsAppeared && !eachEvent.IsFinished && eachEvent.totalEnvironmentDamage > value)
            {
                mostHarmful = eachEvent;
                value = eachEvent.totalEnvironmentDamage;
            }
        }
        return mostHarmful;
    }
    /// <summary>
    /// 获取与指定动物相关事件阶段(报告)
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public static List<EventStage> GetEventInfosRelatedToAnimal(Animal animal) //TODO: optimize this code
    {
        List<EventStage> eventStages = new List<EventStage>();
        instance.events.ForEach(anEvent => eventStages.AddRange(anEvent.GetRevealedStagesRelatedToAnimal(animal)));
        return eventStages;
    }
    /// <summary>
    /// 获取环境相关事件阶段(报告)
    /// </summary>
    /// <returns></returns>
    public static List<EventStage> GetEventInfosRelatedToEnvironment() //TODO: optimize this code
    {
        List<EventStage> eventStages = new List<EventStage>();
        instance.events.ForEach(anEvent => eventStages.AddRange(anEvent.GetRevealedStagesRelatedToEnvironment()));
        return eventStages;
    }
    /// <summary>
    /// 获取可用全局措施
    /// </summary>
    /// <returns></returns>
    public static List<GlobalAction> GetEnabledGlobalActions()
    {
        return instance.includedGlobalActions.FindAll(action => action.enabled());
    }
    /// <summary>
    /// 获取可用地区措施
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static List<AreaAction> GetEnabledAreaActions(Area area)
    {
        return instance.includedAreaActions.FindAll(action => action.enabled(area));
    }
    /// <summary>
    /// 获取可用地区建筑
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static List<BuildingInfo> GetEnabledBuildings(Area area)
    {
        return instance.includedBuildings.FindAll(building => building.CanConstructIn(area));
    }
    /// <summary>
    /// 增加措施执行完成数
    /// </summary>
    /// <param name="action"></param>
    public static void AddActionFinishCount(Action action)
    {
        if (instance.actionsFinishCount.ContainsKey(action))
            instance.actionsFinishCount[action]++;
        else
            instance.actionsFinishCount.Add(action, 1);
    }
    /// <summary>
    /// 获取措施执行完成数
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static int GetActionFinishCount(Action action)
    {
        return instance.actionsFinishCount.ContainsKey(action) ? instance.actionsFinishCount[action] : 0;
    }
    /// <summary>
    /// 获取所有动物
    /// </summary>
    /// <returns></returns>
    public static List<Animal> GetSpecies()
    {
        List<Animal> animals = new List<Animal>();
        foreach (Area area in instance.areas)
        {
            animals.Union(area.GetSpecies());
        }
        return animals;
    }
    private static Animal showingNumberPopsAnimal;
    /// <summary>
    /// 地图上正在标出数量与增减量的动物
    /// </summary>
    public static Animal ShowingNumberPopsAnimal { get { return showingNumberPopsAnimal; } }
    /// <summary>
    /// 所有地块上显示特定动物的数量标记<para></para>
    /// 参数为null时隐藏标记
    /// </summary>
    /// <param name="animal"></param>
    public static void ShowAnimalNumberPop(Animal animal)
    {
        showingNumberPopsAnimal = animal;
        foreach (Area area in instance.areas)
        {
            area.ShowAnimalNumberPop(animal);
        }
    }
}
