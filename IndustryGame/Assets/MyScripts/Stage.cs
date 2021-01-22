using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Stage : MonoBehaviour
{
    private static Stage instance;

    private Area[] areas;
    private Area baseArea;
    private List<Region> regions = new List<Region>();

    public string stageName;
    [TextArea]
    public string description;
    [Header("Money allowed for this stage")]
    public int stageMoney;
    [Header("Time allowed for this stage")]
    public int stageTime;
    public List<Event> events;
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
    private int lestMoney;
    public static int contribution;
    public static int reputation;
    private List<Specialist> specialists = new List<Specialist>();
    private List<GlobalAction> includedGlobalActions = new List<GlobalAction>();
    private List<AreaAction> includedAreaActions = new List<AreaAction>();
    private Dictionary<Action, int> actionsFinishCount = new Dictionary<Action, int>();

    //TODO Delete after test
    private Specialist test = new Specialist();

    void Awake()
    {
        if (instance == null)
            instance = this;
        //set stage money objective
        lestMoney = stageMoney;
        //init events
        foreach(Event anEvent in events) {
            anEvent.init();
        }
        //union actions
        foreach(Event anEvent in events)
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
    }
    void Start()
    {
        //collect all Area Components in children gameObject
        areas = GetComponentsInChildren<Area>();
        //pick random area as basement area
        if (areas.Length > 0)
        {
            baseArea = areas[UnityEngine.Random.Range(0, areas.Length)];
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
                    Debug.Log("Stage: initiating area");
                    // if (area.environmentType == environment)
                    // {
                        area.changeSpeciesAmount(animalInitialAmount.animal, (int)(baseAmount * (0.95 + 0.10 * random.NextDouble())));
                    // }
                }
            }
        }

        // TODO: Delete after testing
        
        test.name = "naomi";
        test.speciality = Ability.Amphibian;
        //test.birthday = "2000-05-11";
        //test.birthplace = "Shanghai";
        //test.specialistTemplate.jender = SpecialistTemplate.Jender.Female;
        //test.abilities.Add(Ability.Outdoor, 1);
        specialists.Insert(0, test);
        //specialists.Insert(1, test);
    }
    void Update()
    {
        Timer.idle();
        if (lastDay != Timer.GetDay()) //day change happened
        {
            lastDay = Timer.GetDay();
            foreach (Area area in areas)
            {
                area.dayIdle();
            }
            foreach(Specialist specialist in specialists)
            {
                specialist.dayIdle();
            }
            foreach (Event eachEvent in events)
            {
                eachEvent.dayIdle();
            }
        }

    }
    public static HexGrid GetHexGrid()
    {
        return instance.hexGrid;
    }

    public static Area[] getAreas()
    {
        return instance.areas;
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
        instance.lestMoney -= value;
    }
    public static int GetLestMoney()
    {
        return instance.lestMoney;
    }
    public static List<Event> GetEvents()
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
