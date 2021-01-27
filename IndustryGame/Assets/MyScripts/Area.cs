using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Area : MonoBehaviour
{
    [SerializeField]
    public GameObject rainFX, snowFX, markSpecialist, markBasement;
    public string areaName;
    [TextArea]
    public string description;
    public EnvironmentType environmentType;

    public readonly List<Building> buildings = new List<Building>();
    [Serializable]
    public struct Stat
    {
        public AreaStat statType;
        public double value;
    }
    public List<Stat> stat;

    public Region region;
    Dictionary<HexDirection, Area> neibors = new Dictionary<HexDirection, Area>();
    private List<AreaAction> finishedActions = new List<AreaAction>();
    private Dictionary<Animal, AmountChange> animalAmounts = new Dictionary<Animal, AmountChange>();
    private LinkedList<Dictionary<Animal, AmountChange>> animalAmountsRecords = new LinkedList<Dictionary<Animal, AmountChange>>();

    // Weather params
    private Weather weather;
    public float totalWater = 1000f;
    public float groundSkyRatio = 0.7f;
    public float rainSnowRatio = 0.4f;
    public float rainFallRatio = 0.3f;
    private void Start()
    {
        environmentType = (EnvironmentType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnvironmentType)).Length);
        if (environmentType.Equals(EnvironmentType.Mountains))
            areaName = Resources.Load<NameTemplates>("NameTemplates/MountainName").pickRandomOne();
        else
            areaName = Resources.Load<NameTemplates>("NameTemplates/PlainName").pickRandomOne();

        HexCell cell = GetHexCell();
        weather = new Weather(cell.Elevation, totalWater, groundSkyRatio, rainSnowRatio, rainFallRatio);

        foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
        {
            HexCell neighborCell = cell.GetNeighbor(direction);
            if (neighborCell != null)
            {
                neibors.Add(direction, neighborCell.transform.GetComponentInChildren<Area>());
            }
        }
        //initial buildings
        foreach (Building building in buildings)
        {
            building.FinishConstruction();
        }
        markBasement.SetActive(isBasement());
    }
    public int getSpeciesAmount(Animal animal)
    {
        return animalAmounts.ContainsKey(animal) ? (int)animalAmounts[animal].old : 0;
    }
    public int getSpeciesChange(Animal animal)
    {
        return animalAmounts.ContainsKey(animal) ? (int)animalAmounts[animal].change : 0;
    }
    public ICollection<Animal> getSpecies()
    {
        return animalAmounts.Keys;
    }

    public List<List<Animal>> getSpeciesDangerTypes()
    {
        List<List<Animal>> animalsDangerList = new List<List<Animal>>();
        int mostDangerType = EnumHelper.GetMaxEnum<SpeciesDangerType>();
        for (int i = 0; i <= mostDangerType; i++)
            animalsDangerList.Add(new List<Animal>());

        InGameLog.AddLog("In getSpeciesDangerTypes " + EnumHelper.GetMaxEnum<SpeciesDangerType>() + " " + animalAmounts.Count);

        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            InGameLog.AddLog("In getSpeciesDangerTypes - foreach");

            int amount = (int)animalAndAmount.Value.old;
            Animal animal = animalAndAmount.Key;
            if (animalAndAmount.Value.old > 0)
            {
                int dangerType = (int)animal.getDangerType(amount);

                InGameLog.AddLog("Adding " + animal.animalName + " to " + dangerType + " " + animalsDangerList.Count);
                if (animalsDangerList[dangerType] == null)
                    animalsDangerList[dangerType] = new List<Animal>();

                animalsDangerList[dangerType].Add(animal);
            }

        }

        return animalsDangerList;
    }

    public void changeSpeciesAmount(Animal animal, int change)
    {
        if (animalAmounts.ContainsKey(animal))
        {
            animalAmounts[animal].Add(change);
        }
        else if (change > 0)
        {
            animalAmounts.Add(animal, new AmountChange(change));
        }
    }
    public void dayIdle()
    {
        // weather.judgeWeather();
        // switch (weather.GetWeatherType())
        // {
        //     case Weather.WeatherType.Rainy:
        //         setWeatherFX(rainFX, true);
        //         setWeatherFX(snowFX, false);
        //         break;
        //     case Weather.WeatherType.Snowy:
        //         setWeatherFX(rainFX, false);
        //         setWeatherFX(snowFX, true);
        //         break;
        //     default:
        //         setWeatherFX(rainFX, false);
        //         setWeatherFX(snowFX, false);
        //         break;
        // }
        foreach (Building building in buildings)
        {
            building.DayIdle();
        }
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            animalAndAmount.Value.recordChange();
            animalAndAmount.Key.idle(this, (int)animalAndAmount.Value.old);
        }
        //show specialist mark // will be upgraded to show count in future
        if (getSpecialistsInArea().Count > 0)
        {
            markSpecialist.SetActive(true);
        }
        else
        {
            markSpecialist.SetActive(false);
        }
    }

    public void setWeatherFX(GameObject weatherFX, bool active)
    {
        // weatherFX
        if (!weatherFX.activeInHierarchy && active)
            weatherFX.SetActive(true);

        Animator animator = weatherFX.GetComponent<Animator>();
        if (animator.gameObject.activeSelf)
        {
            if (active != animator.GetBool("active"))
                animator.SetBool("active", active);
        }
    }

    public List<Specialist> getSpecialistsInArea()
    {
        List<Specialist> list = new List<Specialist>();
        foreach (Specialist specialist in Stage.GetSpecialists())
        {
            Area area = specialist.getCurrentArea();
            if (area == null)
            {
                InGameLog.AddLog("found a specialist not defiend current area", Color.red);
                specialist.moveToArea(Stage.getBaseArea());
            }
            else if (area.Equals(this))
            {
                list.Add(specialist);
            }
        }
        return list;
    }
    public bool isBasement()
    {
        Area area = Stage.getBaseArea();
        return area == null ? false : area.Equals(this);
    }
    public void addReservation()
    {
        Dictionary<Animal, AmountChange> copy = new Dictionary<Animal, AmountChange>(animalAmounts);
        animalAmountsRecords.AddLast(copy);
    }
    public int getSpeciesAmountInLatestRecord(Animal animal)
    {
        Dictionary<Animal, AmountChange> latestRecord = animalAmountsRecords.Last.Value;
        return latestRecord.ContainsKey(animal) ? (int)latestRecord[animal].old : 0;
    }
    public int getSpeciesChangeInLatestRecord(Animal animal)
    {
        Dictionary<Animal, AmountChange> latestRecord = animalAmountsRecords.Last.Value;
        return latestRecord.ContainsKey(animal) ? (int)latestRecord[animal].change : 0;
    }
    public Weather GetWeather()
    {
        return weather;
    }
    public List<AreaAction> GetEnabledActions()
    {
        return Stage.GetEnabledAreaActions(this);
    }
    public List<BuildingInfo> GetEnabledBuildings()
    {
        return Stage.GetEnabledBuildings(this);
    }
    public void AddFinishedAction(AreaAction action)
    {
        finishedActions.Add(action);
    }
    public bool ContainsFinishedAction(AreaAction action)
    {
        return finishedActions.Contains(action);
    }

    public List<AreaAction> GetFinishedActions()
    {
        return finishedActions;
    }
    public void StartConstruction(BuildingInfo buildingInfo)
    {
        buildings.Add(new Building(this, buildingInfo));
        constructionProgressSlider.gameObject.SetActive(true);
    }
    public void StartDeConstruction(Building building)
    {
        buildings.Remove(building);
    }
    public bool ContainsConstructedBuildingInfo(BuildingInfo buildingInfo)
    {
        return buildings.Find(building => building.info.Equals(buildingInfo) && building.IsConstructed()) != null;
    }
    public int CountConstructedBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        buildings.ForEach(building => { if (building.IsConstructed() && building.info.Equals(buildingInfo)) ++count; });
        return count;
    }
    public int CountBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        buildings.ForEach(building => { if (building.info.Equals(buildingInfo)) ++count; });
        return count;
    }
    public ICollection<Area> GetNeighborAreas()
    {
        return neibors.Values;
    }
    public Area GetNeighborArea(HexDirection direction)
    {
        return neibors.ContainsKey(direction) ? neibors[direction] : null;
    }
    //foods - animals as food
    //energyNeedsForOneEater - energy one eater requires
    //units - amount of eater
    //returns - satisfied amount
    public int ProvideFood(List<Animal> foods, int energyNeedsForOneEater, int units)
    {
        if (units <= 0)
        {
            return 0;
        }
        if (energyNeedsForOneEater <= 0)
        { //satisfies all
            return units;
        }
        int sumProvidableAmount = 0;
        bool satisfied = false;
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            if (!foods.Contains(animalAndAmount.Key)) //skip non-food animals
                continue;
            int providableAmount = (int)(animalAndAmount.Value.old * animalAndAmount.Key.energyAsFood / energyNeedsForOneEater);
            if (sumProvidableAmount + providableAmount >= units)
            {
                providableAmount = units - sumProvidableAmount;
                satisfied = true;
            }
            sumProvidableAmount += providableAmount;
            animalAndAmount.Value.Add(-providableAmount); //decrease eaten animals
            if (satisfied)
                break;
        }
        return sumProvidableAmount;
    }
    public int GetProvidableFoodEnergy(List<Animal> foods, int energyNeedsForOneEater)
    {
        if (energyNeedsForOneEater <= 0)
        {
            return 0; // no energy needs means nothing can provide
        }
        int sumProvidableAmount = 0;
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            if (!foods.Contains(animalAndAmount.Key)) //skip non-food animals
                continue;
            int providableAmount = (int)(animalAndAmount.Value.old * animalAndAmount.Key.energyAsFood / energyNeedsForOneEater);
            sumProvidableAmount += providableAmount;
        }
        return sumProvidableAmount;
    }

    // 进度条显示
    public Slider actionProgressSlider, constructionProgressSlider;
    private Specialist currentSpecialist;

    public void StartProgressSlider(Specialist specialist)
    {
        actionProgressSlider.gameObject.SetActive(true);
        actionProgressSlider.value = 0.0f;
        currentSpecialist = specialist;
    }

    public void UpdateProgressSlider()
    {
        if (actionProgressSlider.gameObject.activeSelf)
        {
            if (currentSpecialist.hasCurrentAction())
            {
                actionProgressSlider.value = currentSpecialist.getActionProgressRate();
            }
            else
            {
                actionProgressSlider.gameObject.SetActive(false);
            }
        }
        if (constructionProgressSlider.gameObject.activeSelf)
        {
            Building building = buildings.Find(eachBuilding => !eachBuilding.IsConstructed());

            if (building == null)
            {
                constructionProgressSlider.gameObject.SetActive(false);
            }
            else
            {
                constructionProgressSlider.value = building.GetConstructionRate();
            }
        }
    }

    private void Update()
    {
        UpdateProgressSlider();
    }
    public HexCell GetHexCell()
    {
        return transform.GetComponentInParent<HexCell>();
    }
    public List<Building> GetConstructedBuildings()
    {
        return buildings.FindAll(building => building.IsConstructed());
    }
}