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
    //private static Stage instance;
    public static Stage instance;

    private Area[] areas;
    public List<Region> regions = new List<Region>();

    public string stageName;
    [TextArea]
    public string description;
    public List<MainEventSO> definedEvents;
    public List<MainEvent> events = new List<MainEvent>();
    [Header("Money allowed for this stage")]
    public int stageMoney;
    [Serializable]
    public struct AnimalInitialAmount
    {
        public Animal animal;
        public int amount;
    }
    [Header("初始物种数")]
    public List<AnimalInitialAmount> animalInitialAmounts;
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

    private Stage() { }
    void Awake()
    {
        if (instance == null)
            instance = this;
        //init resources
        foreach(ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            resources.Add(resourceType, new AmountChange(0));
        //set stage money objective
        resources[ResourceType.money].AddWithoutRecord(stageMoney);
        //load actions
        foreach(AreaAction areaAction in Resources.LoadAll<AreaAction>("Action/AreaAction"))
        {
            includedAreaActions.Add(areaAction);
        }
        foreach (GlobalAction globalAction in Resources.LoadAll<GlobalAction>("Action/GlobalAction"))
        {
            includedGlobalActions.Add(globalAction);
        }
        //load buldings
        foreach(BuildingInfo building in Resources.LoadAll<BuildingInfo>("Building"))
        {
            includedBuildings.Add(building);
        }
    }
    public void Init(HexCell[] hexCells)
    {
        //collect all Area Components in children gameObject

        areas = new Area[hexCells.Length];
        for (int i = 0; i < hexCells.Length; ++i)
        {
            areas[i] = hexCells[i].GetComponentInChildren<Area>();
        }

        //generate regions

        foreach(Area area in areas)
        {
            int regionId = area.GetHexCell().RegionId;
           
            Region region = regions.Find(eachRegion => eachRegion.GetRegionId() == regionId);
            // Debug.Log("Region: " + area.region.GetRegionId() + " " + area.transform.position);
            if (region == null)
            {
                regions.Add(region = new Region(regionId));
            }
            region.AddArea(area);
            area.region = region;

        }
        foreach (Region region in regions)
        {
            region.CalculateCenter();
        }
        //generate initial events
        foreach (MainEventSO definedEvent in definedEvents)
        {
            if(definedEvent.onlyGenerateAtBeginning)
            {
                MainEvent mainEvent = definedEvent.TryGenerate();
                if (mainEvent != null)
                    events.Add(mainEvent);
            }
        }
        
        //debug
        //foreach(Region region in regions) {
        //    InGameLog.AddLog("region id " + region.GetRegionId() + " area " + region.GetAreas().Count + " from " + areas.Length);
        //}
        //generate specialist employment list
        SpecialistEmployList.refresh();
        hexGrid = GetComponent<HexGrid>();
        Camera.main.GetComponent<OrthographicCamera>().SetHexGrid(hexGrid);
        lastDay = Timer.GetDay();
        foreach (AnimalInitialAmount animalInitialAmount in animalInitialAmounts)
        {
            EnvironmentType environment = animalInitialAmount.animal.bestEnvironmentType;
            int count = 0;
            foreach(Area area in areas)
            {
                // if(area.environmentType == environment)
                // {
                    ++count;
                // }
            }
            if (count > 0)
            {
                int baseAmount = animalInitialAmount.amount / count;
                System.Random random = new System.Random();
                foreach (Area area in areas)
                {
                    // if (area.environmentType == environment)
                    // {
                        area.changeSpeciesAmount(animalInitialAmount.animal, (int)(baseAmount * (0.95 + 0.10 * random.NextDouble())));
                    // }
                }
            }
        }
        //union actions
        foreach (Region region in regions)
        {
            foreach (MainEvent anEvent in region.GetEvents())
            {
                foreach (GlobalAction action in anEvent.includedGlobalActions)
                {
                    if (!includedGlobalActions.Contains(action))
                        includedGlobalActions.Add(action);
                }
                foreach (AreaAction action in anEvent.includedAreaActions)
                {
                    if (!includedAreaActions.Contains(action))
                        includedAreaActions.Add(action);
                }
            }
        }
    }
    void Update()
    {
        //region reservation animation
        foreach(Region region in regions)
        {
            region.FrameIdle();
        }
        //check time
        Timer.idle();
        if (lastDay != Timer.GetDay()) //day change happened
        {
            lastDay = Timer.GetDay();
            //record resource changes
            foreach (AmountChange amountChange in resources.Values)
                amountChange.recordChange();
            //region dayidle
            foreach (Region region in regions)
            {
                region.dayIdle();
            }
            //specialist dayidle
            foreach (Specialist specialist in specialists)
            {
                specialist.dayIdle();
            }
            //defined events dayIdle(generate etc)
            foreach (MainEventSO so in definedEvents)
            {
                so.DayIdle();
            }
        }
    }

    public static Region GetRegion(Area area)
    {
        int regionId = area.GetHexCell().RegionId;
        Region region = GetRegions().Find(eachRegion => eachRegion.GetRegionId() == regionId);
        return region;
    }

    public static HexGrid GetHexGrid()
    {
        return instance.hexGrid;
    }

    public static Area[] getAreas()
    {
        return instance.areas;
    }
    public static List<Region> GetRegions()
    {
        return instance.regions;
    }
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
    public static List<Animal> GetConcernedSpecies()
    {
        return instance.concernedAnimals;
    }
    public static int GetSpeciesAmount(Animal species)
    {
        int total = 0;
        foreach(Area area in instance.areas) {
            total += area.GetSpeciesAmount(species);
        }
        return total;
    }
    public static int GetSpeciesChange(Animal species)
    {
        int total = 0;
        foreach (Area area in instance.areas)
        {
            total += area.GetSpeciesChange(species);
        }
        return total;
    }
    public static List<Specialist> GetSpecialists()
    {
        return instance.specialists;
    }
    public static void subMoney(int value)
    {
        instance.resources[ResourceType.money].AddWithoutRecord(-value);
    }
    public static int GetLestMoney()
    {
        return (int)instance.resources[ResourceType.money].old;
    }
    public static float GetResourceValue(ResourceType resourceType)
    {
        return instance.resources[resourceType].old;
    }
    public static float AddResourceValue(ResourceType resourceType, float value)
    {
        return instance.resources[resourceType].Add(value);
    }
    public static List<MainEvent> GetEvents()
    {
        return instance.events;
    }
    public static List<MainEvent> GetRevealedEvents()
    {
        return instance.events.FindAll(eachEvent => eachEvent.IsAppeared());
    }
    public static List<MainEvent> GetRevealedUnfinishedEvents()
    {
        return instance.events.FindAll(eachEvent => eachEvent.IsAppeared() && !eachEvent.IsFinished());
    }
    public static List<EventStage> GetEventInfosRelatedToAnimal(Animal animal) //TODO: optimize this code
    {
        List<EventStage> eventStages = new List<EventStage>();
        instance.events.ForEach(anEvent => eventStages.AddRange(anEvent.GetRevealedUnfinishedStagesRelatedToAnimal(animal)));
        return eventStages;
    }
    public static List<EventStage> GetEventInfosRelatedToEnvironment() //TODO: optimize this code
    {
        List<EventStage> eventStages = new List<EventStage>();
        instance.events.ForEach(anEvent => eventStages.AddRange(anEvent.GetRevealedUnfinishedStagesRelatedToEnvironment()));
        return eventStages;
    }
    public static List<GlobalAction> GetEnabledGlobalActions()
    {
        return instance.includedGlobalActions.FindAll(action => action.enabled());
    }
    public static List<AreaAction> GetEnabledAreaActions(Area area)
    {
        return instance.includedAreaActions.FindAll(action => action.enabled(area));
    }
    public static List<BuildingInfo> GetEnabledBuildings(Area area)
    {
        return instance.includedBuildings.FindAll(building => building.enabled(area));
    }
    public static void AddActionFinishCount(Action action)
    {
        if (instance.actionsFinishCount.ContainsKey(action))
            instance.actionsFinishCount[action]++;
        else
            instance.actionsFinishCount.Add(action, 1);
    }
    public static int GetActionFinishCount(Action action)
    {
        return instance.actionsFinishCount.ContainsKey(action) ? instance.actionsFinishCount[action] : 0;
    }
    public static List<Animal> GetSpecies()
    {
        List<Animal> animals = new List<Animal>();
        foreach(Area area in instance.areas)
        {
            animals.Union(area.GetSpecies());
        }
        return animals;
    }
}
