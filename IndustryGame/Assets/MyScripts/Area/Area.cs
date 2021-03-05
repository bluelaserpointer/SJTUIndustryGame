using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Area : MonoBehaviour
{
    [SerializeField]
    public GameObject rainFX, snowFX, markSpecialist;
    [HideInInspector] public string areaName;
    [TextArea]
    public string description;
    public EnvironmentType environmentType;

    public readonly List<Building> buildings = new List<Building>();
    private List<EnvironmentStat> environmentStats = new List<EnvironmentStat>();

    public Region region;
    Dictionary<HexDirection, Area> neibors = new Dictionary<HexDirection, Area>();
    private List<AreaAction> finishedActions = new List<AreaAction>();
    private Dictionary<Animal, AmountChange> animalAmounts = new Dictionary<Animal, AmountChange>();
    private Dictionary<Animal, AmountChangeRecords> animalRecords = new Dictionary<Animal, AmountChangeRecords>();

    // Weather params
    private Weather weather;
    public float totalWater = 1000f;
    public float groundSkyRatio = 0.7f;
    public float rainSnowRatio = 0.4f;
    public float rainFallRatio = 0.3f;

    //HUD
    public GameObject animalNumberPop;
    public GameObject animalNumberTooltip;
    public SurroundWithUI basementLabelHolder;
    public SurroundWithUI specialistActionButtonsHolder;

    //data
    [HideInInspector]
    public GameObject[] animalPrefabs;

    [Header("正增长取色")]
    public Color IncreaseColor;
    [Header("负增长取色")]
    public Color DecreaseColor;
    [Header("灭绝取色")]
    public Color ExtinctColor;
    [Header("安全取色")]
    public Color SafeColor;
    private void Start()
    {
        areaName = (environmentType = EnvironmentType.PickRandomOne()).usingNameTemplates.PickRandomOne();

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
        animalNumberPop.SetActive(false);
        markSpecialist.SetActive(false);
        //HUD
        /* //TEST: randomly show the action buttons
        if (UnityEngine.Random.Range(0, 25) == 0)
        {
            ShowSpecialistActionButtons(null);
        }
        */
    }
    /// <summary>
    /// 获取指定动物数量
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int GetSpeciesAmount(Animal animal)
    {
        return animalAmounts.ContainsKey(animal) ? (int)animalAmounts[animal].GetRecordValue() : 0;
    }
    /// <summary>
    /// 获取指定动物增减量
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int GetSpeciesChange(Animal animal)
    {
        return animalAmounts.ContainsKey(animal) ? (int)animalAmounts[animal].change : 0;
    }
    /// <summary>
    /// 获取当地动物列表
    /// </summary>
    /// <returns></returns>
    public ICollection<Animal> GetSpecies()
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

            int amount = (int)animalAndAmount.Value.GetRecordValue();
            Animal animal = animalAndAmount.Key;
            if (animalAndAmount.Value.GetRecordValue() > 0)
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
    /// <summary>
    /// 更改动物数量
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="change"></param>
    public void changeSpeciesAmount(Animal animal, int change)
    {
        if (animalAmounts.ContainsKey(animal))
        {
            float oldValue = animalAmounts[animal].GetCurrentValue();
            float newValue = animalAmounts[animal].Add(change);
            if (newValue < oldValue)
            {
                animal.relatedMainEvent.generatedInstance.totalEnvironmentDamage += oldValue - newValue;
            }
        }
        else if (change > 0)
        {
            animalAmounts.Add(animal, new AmountChange(change));
        }
    }
    /// <summary>
    /// 每日流程
    /// </summary>
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
            animalAndAmount.Value.RecordChange();
            animalAndAmount.Key.idle(this, (int)animalAndAmount.Value.GetRecordValue());
        }
        //show specialist mark // will be upgraded to show count in future
        if (GetSpecialistsInArea().Count > 0)
        {
            markSpecialist.SetActive(true);
        }
        else
        {
            markSpecialist.SetActive(false);
        }
        //environment effect
        environmentStats.ForEach(stat => stat.DayIdle());
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
    /// <summary>
    /// 获取在当地的专家列表
    /// </summary>
    /// <returns></returns>
    public List<Specialist> GetSpecialistsInArea()
    {
        return Stage.GetSpecialists().FindAll(specialist => Equals(specialist.GetCurrentArea()));
    }
    /// <summary>
    /// 是否有基地
    /// </summary>
    /// <returns></returns>
    public bool IsBasement()
    {
        return Equals(region.GetBaseArea());
    }
    /// <summary>
    /// 添加动物统计记录
    /// </summary>
    public void AddReservation()
    {
        foreach (var pair in animalAmounts)
        {
            Animal animal = pair.Key;
            if (region.GetConcernedSpecies().Contains(animal)) //only reservate concerned animals
            {
                if (animalRecords.ContainsKey(animal))
                {
                    animalRecords[animal].AddRecord(pair.Value);
                }
                else
                {
                    animalRecords.Add(animal, new AmountChangeRecords(pair.Value));
                }
            }
        }
    }
    /// <summary>
    /// 显示特定动物的数量标记
    /// </summary>
    /// <param name="animal"></param>
    public void ShowAnimalNumberPop(Animal animal)
    {
        if (animal == null || !animalRecords.ContainsKey(animal))
        {
            animalNumberPop.SetActive(false);
            animalNumberTooltip.SetActive(false);
            return;
        }
        int? amount = animalRecords[animal].GetAmountInLatestRecord();
        int? change = animalRecords[animal].GetChangeInLatestRecord();
        string animalNumberStr = amount.HasValue ? amount.ToString() : "-";
        string animalChangeStr = change.HasValue ? (change.Value > 0 ? "+" + change.ToString() : change.ToString()) : "+0";
        Color backgroundColor;
        if (amount == 0)
        {
            backgroundColor = ExtinctColor;
        }
        else if (change < 0)
        {
            backgroundColor = DecreaseColor;
        }
        else if (change > 0)
        {
            backgroundColor = IncreaseColor;
        }
        else
        {
            backgroundColor = SafeColor;
        }
        animalNumberPop.gameObject.GetComponentInChildren<Image>().color = backgroundColor;
        animalNumberPop.gameObject.GetComponentInChildren<Text>().text = animalNumberStr + "\n" + animalChangeStr;
        animalNumberPop.SetActive(true);
        animalNumberTooltip.gameObject.GetComponentInChildren<Text>().text = "现有个体数: " + animalNumberStr + ", 距上次统计变化" + animalChangeStr;
    }
    /// <summary>
    /// 获取最新统计下指定动物数量
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int? GetSpeciesAmountInLatestRecord(Animal animal)
    {
        return animalRecords.ContainsKey(animal) ? animalRecords[animal].GetAmountInLatestRecord() : null;
    }
    /// <summary>
    /// 获取最新统计下指定动物增减量
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int? GetSpeciesChangeInLatestRecord(Animal animal)
    {
        return animalRecords.ContainsKey(animal) ? animalRecords[animal].GetChangeInLatestRecord() : null;
    }
    /// <summary>
    /// 获取天气
    /// </summary>
    /// <returns></returns>
    public Weather GetWeather()
    {
        return weather;
    }
    /// <summary>
    /// 获取可用措施列表
    /// </summary>
    /// <returns></returns>
    public List<AreaAction> GetEnabledActions()
    {
        return Stage.GetEnabledAreaActions(this);
    }
    /// <summary>
    /// 获取可用建筑列表
    /// </summary>
    /// <returns></returns>
    public List<BuildingInfo> GetEnabledBuildings()
    {
        return Stage.GetEnabledBuildings(this);
    }
    /// <summary>
    /// 添加已完成措施记录
    /// </summary>
    /// <param name="action"></param>
    public void AddFinishedAction(AreaAction action)
    {
        finishedActions.Add(action);
    }
    /// <summary>
    /// 是否已完成指定措施
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool ContainsFinishedAction(AreaAction action)
    {
        return finishedActions.Contains(action);
    }
    /// <summary>
    /// 获取已完成措施
    /// </summary>
    /// <returns></returns>
    public List<AreaAction> GetFinishedActions()
    {
        return finishedActions;
    }
    /// <summary>
    /// 开始建造建筑物
    /// </summary>
    /// <param name="buildingInfo"></param>
    public void StartConstruction(BuildingInfo buildingInfo)
    {
        buildings.Add(new Building(buildingInfo, this));
        Stage.AddResourceValue(ResourceType.money, -buildingInfo.moneyCost);
        constructionProgressSlider.gameObject.SetActive(true);
    }
    /// <summary>
    /// 开始拆除建筑物
    /// </summary>
    /// <param name="building"></param>
    public void StartDeConstruction(Building building)
    {
        buildings.Remove(building);
        building.Removed();
    }
    /// <summary>
    /// 是否已建设指定建筑物类型
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
    public bool ContainsConstructedBuildingInfo(BuildingInfo buildingInfo)
    {
        return buildings.Find(building => building.info.Equals(buildingInfo) && building.IsConstructed()) != null;
    }
    /// <summary>
    /// 统计指定建筑物已建设数
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
    public int CountConstructedBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        buildings.ForEach(building => { if (building.IsConstructed() && building.info.Equals(buildingInfo)) ++count; });
        return count;
    }
    /// <summary>
    /// 获取建筑物列表
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
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
    /// <summary>
    /// 消耗猎物
    /// </summary>
    /// <param name="foods">animals as food</param>
    /// <param name="energyNeedsForOneEater">energy one eater requires</param>
    /// <param name="units">amount of eater</param>
    /// <returns>satisfied amount</returns>
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
            int providableAmount = (int)(animalAndAmount.Value.GetRecordValue() * animalAndAmount.Key.energyAsFood / energyNeedsForOneEater);
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
    /// <summary>
    /// 获取指定猎物列表下提供的总能量
    /// </summary>
    /// <param name="foods"></param>
    /// <param name="energyNeedsForOneEater"></param>
    /// <returns></returns>
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
            int providableAmount = (int)(animalAndAmount.Value.GetRecordValue() * animalAndAmount.Key.energyAsFood / energyNeedsForOneEater);
            sumProvidableAmount += providableAmount;
        }
        return sumProvidableAmount;
    }

    // 进度条显示
    [SerializeField] private Slider actionProgressSlider, constructionProgressSlider;
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
            if (currentSpecialist != null && currentSpecialist.HasCurrentAction())
            {
                actionProgressSlider.value = currentSpecialist.GetActionProgressRate();
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
    /// <summary>
    /// 获取已建设建筑列表
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <param name="value"></param>
    public List<Building> GetConstructedBuildings()
    {
        return buildings.FindAll(building => building.IsConstructed());
    }
    /// <summary>
    /// 获取指定环境指标
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <param name="value"></param>
    public float GetEnviromentStat(EnvironmentStatType environmentStatType)
    {
        EnvironmentStat stat = environmentStats.Find(eachStat => eachStat.IsType(environmentStatType));
        return stat == null ? 0 : stat.value;
    }
    /// <summary>
    /// 获取指定环境指标
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <param name="value"></param>
    public float GetEnviromentStatWithString(string environmentStatType)
    {
        EnvironmentStat stat = environmentStats.Find(eachStat => eachStat.name.Equals(environmentStatType));
        return stat == null ? 0 : stat.value;
    }
    /// <summary>
    /// 获取所有负面环境指标
    /// </summary>
    /// <returns></returns>
    public List<EnvironmentStat> GetNegativeEnvironmentStats()
    {
        return environmentStats.FindAll(eachStat => eachStat.isNegative);
    }
    /// <summary>
    /// 是否含任何负面环境指标
    /// </summary>
    /// <returns></returns>
    public bool HasNegativeEnvironmentStats()
    {
        return GetNegativeEnvironmentStats().Count == 0;
    }
    /// <summary>
    /// 生成指定环境指标（取值参照该种类初始值设定）
    /// </summary>
    /// <param name="environmentStatType"></param>
    public void AddEnvironmentStat(EnvironmentStatType environmentStatType)
    {
        float initialvalue = UnityEngine.Random.Range(environmentStatType.initialValueRange.x, environmentStatType.initialValueRange.y);
        AddEnvironmentStat(environmentStatType, initialvalue);
    }
    /// <summary>
    /// 增加指定环境指标
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <param name="value"></param>
    public void AddEnvironmentStat(EnvironmentStatType environmentStatType, float value)
    {
        EnvironmentStat stat = environmentStats.Find(eachStat => eachStat.IsType(environmentStatType));
        if (stat != null)
        {
            stat.value = Mathf.Clamp(stat.value + value, 0.0f, 1.0f);
        }
        else
        {
            stat = new EnvironmentStat(environmentStatType, this);
            stat.value = Mathf.Clamp(stat.value + value, 0.0f, 1.0f);
            environmentStats.Add(stat);
        }
    }
    /// <summary>
    /// 世界地图中的坐标
    /// </summary>
    public Vector3 worldPosition { get { return gameObject.transform.parent.position; } }
    /// <summary>
    /// 洲中的X坐标
    /// </summary>
    public float regionPositionX { get { return worldPosition.x - region.GetLeft(); } }
    /// <summary>
    /// 洲中的Y坐标
    /// </summary>
    public float regionPositionY { get { return worldPosition.z - region.GetTop(); } }
    public void ShowSpecialistActionButtons(Specialist specialist)
    {
        specialistActionButtonsHolder.gameObject.SetActive(true);
        Dictionary<string, UnityAction> buttonNameAndEvent = new Dictionary<string, UnityAction>();
        //public actions
        buttonNameAndEvent.Add("调查该洲", () => Debug.Log("调查该洲"));
        buttonNameAndEvent.Add("集中观察", () => Debug.Log("集中观察"));
        buttonNameAndEvent.Add("管理研究", () => Debug.Log("管理研究"));
        //generate
        GameObject origin = Resources.Load<GameObject>("UI/Area/HexButton");
        List<GameObject> buttons = new List<GameObject>();
        foreach (var nameAndEvent in buttonNameAndEvent)
        {
            GameObject copy = Instantiate(origin);
            copy.transform.parent = HUDManager.instance.transform;
            copy.GetComponentInChildren<Text>().text = nameAndEvent.Key;
            copy.GetComponentInChildren<Button>().onClick.AddListener(nameAndEvent.Value);
            buttons.Add(copy);
        }
        specialistActionButtonsHolder.DestroySurrounders();
        specialistActionButtonsHolder.AddSurrounders(buttons);
    }
    public void HideSpecialistActionButtons()
    {
        specialistActionButtonsHolder.gameObject.SetActive(false);
    }
}