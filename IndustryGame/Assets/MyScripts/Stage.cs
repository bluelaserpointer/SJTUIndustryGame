﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public struct AmountChange
{
    public float old;
    private float current;
    public float change;
    public AmountChange(float initialValue) { old = current = initialValue; change = 0; }
    public float Add(float value) { return current += value; }
    public float AddWithClamp(float value, float min, float max) { return current = Mathf.Clamp(current + value, min, max); }
    public float AddWithoutRecord(float value) { old += value; return current += value; }
    public void recordChange()
    {
        if (current < 0)
            current = 0;
        change = current - old;
        old = current;
    }
}
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
    private Area baseArea;
    public List<Region> regions = new List<Region>();

    public string stageName;
    [TextArea]
    public string description;
    [Header("Money allowed for this stage")]
    public int stageMoney;
    [Header("Time allowed for this stage")]
    public int stageTime;
    public List<MainEvent> events;
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


    void Awake()
    {
        if (instance == null)
            instance = this;
        //init resources
        foreach(ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            resources.Add(resourceType, new AmountChange());
        //set stage money objective
        resources[ResourceType.money].AddWithoutRecord(stageMoney);
        //init events
        foreach (MainEvent anEvent in events) {
            anEvent.init();
        }
        //load actions
        foreach(AreaAction areaAction in Resources.LoadAll<AreaAction>("Action/AreaAction"))
        {
            includedAreaActions.Add(areaAction);
        }
        foreach (GlobalAction globalAction in Resources.LoadAll<GlobalAction>("Action/GlobalAction"))
        {
            includedGlobalActions.Add(globalAction);
        }
        //union actions
        foreach (MainEvent anEvent in events)
        {
            foreach(GlobalAction action in anEvent.includedGlobalActions)
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
        //load buldings
        foreach(BuildingInfo building in Resources.LoadAll<BuildingInfo>("Building"))
        {
            includedBuildings.Add(building);
        }
    }
    public void Init(HexCell[] hexCells)
    {
        //collect all Area Components in children gameObject
        areas = GetComponentsInChildren<Area>();
        //generate regions
        Debug.Log("Areas:" + areas.Length);
        foreach(Area area in areas)
        {
            int regionId = area.GetHexCell().RegionId;
           
            Region region = regions.Find(eachRegion => eachRegion.GetRegionId() == regionId);
            if (region == null)
            {
                regions.Add(region = new Region(regionId));
            }
            region.AddArea(area);
            area.region = region;
        }


        //debug
        //foreach(Region region in regions) {
        //    InGameLog.AddLog("region id " + region.GetRegionId() + " area " + region.GetAreas().Count + " from " + areas.Length);
        //}
        //pick random area as basement area
        if (areas.Length > 0)
        {
            baseArea = areas[UnityEngine.Random.Range(0, areas.Length)];
            regions[0].SetBaseArea(baseArea);
        }
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

    }
    void Update()
    {
        //record resource changes
        foreach (AmountChange amountChange in resources.Values)
            amountChange.recordChange();
        //check time
        Timer.idle();
        if (lastDay != Timer.GetDay()) //day change happened
        {
            lastDay = Timer.GetDay();
            foreach (Region region in regions)
            {
                region.dayIdle();
            }
            foreach(Specialist specialist in specialists)
            {
                specialist.dayIdle();
            }
            foreach (MainEvent eachEvent in events)
            {
                eachEvent.dayIdle();
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
    public static int getSpeciesAmount(Animal species)
    {
        int total = 0;
        foreach(Area area in instance.areas) {
            total += area.getSpeciesAmount(species);
        }
        return total;
    }
    public static int getSpeciesChange(Animal species)
    {
        int total = 0;
        foreach (Area area in instance.areas)
        {
            total += area.getSpeciesChange(species);
        }
        return total;
    }
    public static Area getBaseArea()
    {
        return instance.baseArea;
    }
    public static List<Specialist> GetSpecialists()
    {
        return instance.specialists;
    }
    public static void subMoney(int value)
    {
        instance.resources[ResourceType.money].Add(-value);
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
    public static List<EventInfo> GetEventInfosRelatedToAnimal(Animal animal) //TODO: optimize this code
    {
        List<EventInfo> eventInfos = new List<EventInfo>();
        instance.events.ForEach(anEvent => eventInfos.AddRange(anEvent.GetRevealedInfosRelatedToAnimal(animal)));
        return eventInfos;
    }
    public static List<EventInfo> GetEventInfosRelatedToEnvironment() //TODO: optimize this code
    {
        List<EventInfo> eventInfos = new List<EventInfo>();
        instance.events.ForEach(anEvent => eventInfos.AddRange(anEvent.GetRevealedInfosRelatedToEnvironment()));
        return eventInfos;
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
            animals.Union(area.getSpecies());
        }
        return animals;
    }
}
