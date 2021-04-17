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
    [HideInInspector]
    public string description;
    [HideInInspector]
    public EnvironmentType environmentType;

    public readonly List<Building> buildings = new List<Building>();
    public readonly List<EnvironmentStatFactor> environmentStatFactors = new List<EnvironmentStatFactor>();
    public Habitat habitat;

    public Region region;
    private readonly Dictionary<HexDirection, Area> neibors = new Dictionary<HexDirection, Area>();
    private readonly List<AreaAction> finishedActions = new List<AreaAction>();
    private readonly Dictionary<Animal, AmountChange> animalAmounts = new Dictionary<Animal, AmountChange>();
    private readonly Dictionary<Animal, AmountChangeRecords> animalRecords = new Dictionary<Animal, AmountChangeRecords>();
    private int lastRecordedDay;
    public int DaysFromLastRecord { get { return lastRecordedDay; } }

    // Weather params
    private Weather weather;
    public float totalWater = 1000f;
    public float groundSkyRatio = 0.7f;
    public float rainSnowRatio = 0.4f;
    public float rainFallRatio = 0.3f;

    //HUD
    public Image habitatMarkImage;
    public Image habitatHealthImage;
    public Image habitatInvisibleImage;
    [SerializeField] private Image environmentFactorMarkImage;
    public GameObject animalNumberPop;
    public Text mainTooltipText;
    public SurroundWithUI basementLabelHolder;
    [SerializeField] private SurroundWithUI specialistActionButtonsHolder;
    private static Area showingSpecialistActionButtonsArea;

    //data
    //[HideInInspector]
    public GameObject[] animalPrefabs;
    //[HideInInspector]
    //public GameObject[] expertPrefabs;

    [Header("正增长取色")]
    public Color IncreaseColor;
    [Header("负增长取色")]
    public Color DecreaseColor;
    [Header("灭绝取色")]
    public Color ExtinctColor;
    [Header("安全取色")]
    public Color SafeColor;

    private static GameObject _hexButton;
    private static GameObject hexButton
    {
        get
        {
            if (_hexButton == null)
            {
                _hexButton = Resources.Load<GameObject>("UI/Area/HexButton");
            }
            return _hexButton;
        }
    }

    public string TooltipDescription
    {
        get
        {
            string description = "";
            if (habitat != null && habitat.IsRevealed)
                description += habitat.TooltipDescription + "\n";
            foreach (EnvironmentStatFactor factor in environmentStatFactors)
            {
                description += factor.TooltipDescription + "\n";
            }
            return description;
        }
    }

    private void Start()
    {
        areaName = (environmentType = EnvironmentType.PickRandomOne()).usingNameTemplates.PickRandomOne();

        HexCell cell = GetHexCell();
        weather = new Weather(cell.Elevation, totalWater, groundSkyRatio, rainSnowRatio, rainFallRatio);

        //initial buildings
        foreach (Building building in buildings)
        {
            building.FinishConstruction();
        }
        //HUD
        habitatMarkImage.gameObject.SetActive(false);
        environmentFactorMarkImage.gameObject.SetActive(false);
        animalNumberPop.SetActive(false);
        markSpecialist.SetActive(false);
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
        HexCell cell = GetHexCell();
        cell.BuildingPrefabs.Clear();
        foreach (Building building in buildings)
        {

            building.DayIdle();
        }
        cell.Refresh();
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            animalAndAmount.Value.RecordChange();
            animalAndAmount.Key.idle(this, (int)animalAndAmount.Value.GetRecordValue());
        }
        //show specialist mark // will be upgraded to show count in future
        if (Specialists.Count > 0)
        {
            markSpecialist.SetActive(true);
        }
        else
        {
            markSpecialist.SetActive(false);
        }
        //colony
        if (habitat != null)
            habitat.DayIdle();
        //environment effect factors
        new List<EnvironmentStatFactor>(environmentStatFactors).ForEach(stat => stat.DayIdle());
        List<EnvironmentStatFactor> revealedFactors = environmentStatFactors.FindAll(factor => factor.IsRevealed);
        //HUD
        if (revealedFactors.Count == 0)
        {
            environmentFactorMarkImage.gameObject.SetActive(false);
        }
        else
        {
            float sumHabitabilityAffect = SumHabitabilityAffect(revealedFactors);
            if (sumHabitabilityAffect < 0)
                environmentFactorMarkImage.color = Color.Lerp(Color.red, Color.white, sumHabitabilityAffect * 2 + 1.0f);
            else
                environmentFactorMarkImage.color = Color.Lerp(Color.white, Color.green, sumHabitabilityAffect * 2);
            environmentFactorMarkImage.gameObject.SetActive(true);
        }
        mainTooltipText.text = TooltipDescription;
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
    public List<Specialist> Specialists { get { return Stage.GetSpecialists().FindAll(specialist => Equals(specialist.Area)); } }
    /// <summary>
    /// 是否为基地
    /// </summary>
    public bool IsBasement { get { return Equals(region.GetBaseArea()); } }
    /// <summary>
    /// 添加动物统计记录,同时揭开当地的栖息地
    /// </summary>
    public void AddReservation()
    {
        if (habitat != null)
            habitat.Reveal();
        foreach (EnvironmentStatFactor environmentStatFactor in environmentStatFactors)
        {
            environmentStatFactor.Reveal();
        }
        lastRecordedDay = Timer.TotalDaysPassed;
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
        //update number pops
        if (Stage.ShowingNumberPopsAnimal != null)
        {
            Stage.ShowAnimalNumberPop(Stage.ShowingNumberPopsAnimal);
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
            return;
        }
        int? amount = animalRecords[animal].GetAmountInLatestRecord();
        int? change = animalRecords[animal].GetChangeInLatestRecord();
        string animalNumberStr = amount.HasValue ? amount.ToString() : "-";
        string animalChangeStr = change.HasValue ? (change.Value > 0 ? "+" + change.ToString() : change.ToString()) : "+0";
        // change pop color depends on amount increse/decrese
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
        // change brightness depends on days passed from last record
        float darkness = 1 - Mathf.Clamp(DaysFromLastRecord / 30f, 0, 1);
        backgroundColor = Color.Lerp(backgroundColor, Color.black, darkness);
        animalNumberPop.gameObject.GetComponentInChildren<Image>().color = backgroundColor;
        animalNumberPop.gameObject.GetComponentInChildren<Text>().text = animalNumberStr + "\n" + animalChangeStr;
        animalNumberPop.SetActive(true);
        //mainTooltipText.text = "现有个体数: " + animalNumberStr + ", 距上次统计变化" + animalChangeStr;
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
        if (buildings.Count >= 3 || !Stage.TryAddResourceValue(ResourceType.money, -buildingInfo.moneyCost)) return;

        GameObject.FindGameObjectWithTag("BuildingSfxPlayer").GetComponent<BuildingSfxPlayer>().StartConstruction();
        buildings.Add(new Building(buildingInfo, this));
        constructionProgressSlider.gameObject.SetActive(true);
    }
    /// <summary>
    /// 开始拆除建筑物
    /// </summary>
    /// <param name="building"></param>
    public void StartDeConstruction(Building building)
    {
        GameObject.FindGameObjectWithTag("BuildingSfxPlayer").GetComponent<BuildingSfxPlayer>().StartConstruction();
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
        if (neibors.Values.Count == 0)
        {
            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                HexCell neighborCell = GetHexCell().GetNeighbor(direction);
                if (neighborCell != null)
                {
                    neibors.Add(direction, neighborCell.transform.GetComponentInChildren<Area>());
                }
            }
        }
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
    public Specialist currentSpecialist;

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
            if (currentSpecialist != null)
            {
                actionProgressSlider.value = currentSpecialist.GetActionProgressRate();
            }
            else
            {
                currentSpecialist = null;
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
    public float GetEnviromentStatFactor(EnvironmentStatType environmentStatType)
    {
        EnvironmentStatFactor factor = environmentStatFactors.Find(eachStat => eachStat.IsType(environmentStatType));
        return factor == null ? 0 : factor.FactorValue;
    }
    /// <summary>
    /// 获取指定环境指标
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <param name="value"></param>
    public float GetEnviromentStatWithString(string environmentStatType)
    {
        EnvironmentStatFactor factor = environmentStatFactors.Find(eachStat => eachStat.Name.Equals(environmentStatType));
        return factor == null ? 0 : factor.FactorValue;
    }
    /// <summary>
    /// 生成指定环境指标, 取值参照该种类初始值设定
    /// </summary>
    /// <param name="environmentStatType"></param>
    public void AddEnvironmentFactor(EnvironmentStatType environmentStatType)
    {
        EnvironmentStatFactor stat = environmentStatFactors.Find(eachStat => eachStat.IsType(environmentStatType));
        if (stat == null)
        {
            environmentStatFactors.Add(new EnvironmentStatFactor(environmentStatType, this));
        }
    }
    public EnvironmentStatFactor ChangeEnvironmentFactorAffection(EnvironmentStatType environmentStatType, float value)
    {
        EnvironmentStatFactor factor = environmentStatFactors.Find(eachStat => eachStat.IsType(environmentStatType));
        if (factor != null)
        {
            factor.FactorValue += value;
            return factor;
        }
        return null;
    }
    public float CalcurateHabitability()
    {
        //TODO: calcurate environmentFactors farther than distance 1
        float habitability = 0.5f;
        foreach (EnvironmentStatFactor factor in environmentStatFactors)
        {
            habitability += factor.ReceiveAffect(0);
        }
        foreach (Area eachArea in GetNeighborAreas())
        {
            foreach (EnvironmentStatFactor factor in eachArea.environmentStatFactors)
            {
                habitability += factor.ReceiveAffect(1);
            }
        }
        return habitability;
    }
    public static float SumHabitabilityAffect(List<EnvironmentStatFactor> factors)
    {
        float sumAffect = 0;
        factors.ForEach(factor => sumAffect += factor.SeekAffect(0));
        return sumAffect;
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
    /// <summary>
    /// 显示建设确认按钮列表（只有确认/取消）
    /// </summary>
    public void ShowConstructionButtons(BuildingInfo buildingInfo)
    {
        if (showingSpecialistActionButtonsArea != null)
            showingSpecialistActionButtonsArea.HideSpecialistActionButtons();
        showingSpecialistActionButtonsArea = this;
        Dictionary<string, UnityAction> buttonNameAndEvent = new Dictionary<string, UnityAction>();
        buttonNameAndEvent.Add("取消", () => { });
        if(buildingInfo.CanConstructIn(this))
            buttonNameAndEvent.Add("建设:" + buildingInfo.moneyCost + "$", () => { StartConstruction(buildingInfo); });
        //generate
        List<GameObject> buttons = new List<GameObject>();
        foreach (var nameAndEvent in buttonNameAndEvent)
        {
            GameObject copy = Instantiate(hexButton);
            copy.transform.parent = Stage.CanvasForSurroundersHUD.transform;
            copy.GetComponentInChildren<Text>().text = nameAndEvent.Key;
            Button button = copy.GetComponentInChildren<Button>();
            button.onClick.AddListener(nameAndEvent.Value);
            button.onClick.AddListener(() => HideSpecialistActionButtons());
            buttons.Add(copy);
        }
        specialistActionButtonsHolder.DestroySurrounders();
        specialistActionButtonsHolder.AddSurrounders(buttons);
    }
    /// <summary>
    /// 显示专家措施按钮列表
    /// </summary>
    /// <param name="specialist"></param>
    public void ShowSpecialistActionButtons(Specialist specialist)
    {
        if (showingSpecialistActionButtonsArea != null)
            showingSpecialistActionButtonsArea.HideSpecialistActionButtons();
        showingSpecialistActionButtonsArea = this;
        Dictionary<string, UnityAction> buttonNameAndEvent = new Dictionary<string, UnityAction>();
        //cancel
        buttonNameAndEvent.Add("取消", () => { });
        //watch habitat
        if (habitat != null && habitat.IsRevealed)
            buttonNameAndEvent.Add("观察栖息地", () => specialist.SetAction(new WatchHabitat(specialist, this)));
        //work with environment problem
        foreach (EnvironmentStatFactor factor in environmentStatFactors)
        {
            if (factor.IsRevealed && factor.DayValueChangeBySpecialistAction != 0)
            {
                buttonNameAndEvent.Add("对策" + factor.Name, () => specialist.SetAction(new WorkEnvironmentProblem(specialist, factor)));
            }
        }
        //work with building
        foreach (Building building in buildings)
        {
            if (building.info.provideSpecialistAction)
            {
                buttonNameAndEvent.Add(building.info.buildingName, () => specialist.SetAction(new BoostBuildingEffects(specialist, building)));
            }
        }
        //generate above buttons
        List<GameObject> buttons = new List<GameObject>();
        foreach (var nameAndEvent in buttonNameAndEvent)
        {
            GameObject copy = Instantiate(hexButton);
            copy.transform.parent = Stage.CanvasForSurroundersHUD.transform;
            copy.GetComponentInChildren<Text>().text = nameAndEvent.Key;
            Button button = copy.GetComponentInChildren<Button>();
            button.onClick.AddListener(nameAndEvent.Value);
            button.onClick.AddListener(() => HideSpecialistActionButtons());
            buttons.Add(copy);
        }
        specialistActionButtonsHolder.DestroySurrounders();
        specialistActionButtonsHolder.AddSurrounders(buttons);
    }
    public void HideSpecialistActionButtons()
    {
        showingSpecialistActionButtonsArea = null;
        specialistActionButtonsHolder.DestroySurrounders();
    }
}