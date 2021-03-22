using UnityEngine;

public class Habitat
{
    private static readonly Sprite[] habitatSprites = new Sprite[6];
    private static readonly Sprite[] unknownHabitatSprites = new Sprite[6];
    static Habitat()
    {
        for(int i = 0; i <= 5; ++i)
        {
            habitatSprites[i] = Resources.Load<Sprite>("UI/Area/Habitat" + i);
            unknownHabitatSprites[i] = Resources.Load<Sprite>("UI/Area/UnknownHabitat" + i);
        }
    }
    public Habitat(Area area, Animal animal, int level)
    {
        (this.area = area).habitat = this;
        this.animal = animal;
        int minPopulation = animal.minHabitatPopulation(level);
        int maxPopulation = animal.maxHabitatPopulation(level);
        amount = UnityEngine.Random.Range(minPopulation, maxPopulation);

    }
    private Area area;
    /// <summary>
    /// 栖息地所在地点
    /// </summary>
    public Area Area { get { return area; } }
    /// <summary>
    /// 栖息动物
    /// </summary>
    public readonly Animal animal;
    private float amount;
    /// <summary>
    /// 栖息数量
    /// </summary>
    public float Amount { get { return amount; } }
    /// <summary>
    /// 栖息地规模(0 ~ 5)
    /// </summary>
    public int Level {
        get {
            if (amount < animal.habitat1MinPopulation) return 0;
            if (amount < animal.habitat2MinPopulation) return 1;
            if (amount < animal.habitat3MinPopulation) return 2;
            if (amount < animal.habitat4MinPopulation) return 3;
            if (amount < animal.habitat5MinPopulation) return 4;
            return 5;
        }
    }
    private float lastCheckedHabitability = 0.5f;
    /// <summary>
    /// 最后确认适居性
    /// </summary>
    public float LastCheckedHabitability { get { return lastCheckedHabitability; } }
    private int lastCheckedLevel;
    /// <summary>
    /// 最后确认等级
    /// </summary>
    public int LastCheckedLevel { get { return lastCheckedLevel; } }
    private float habitability = 0.5f;
    /// <summary>
    /// 适居性
    /// </summary>
    public float Habitability { get { return habitability; } }
    private bool isRevealed;
    /// <summary>
    /// 是否已发现
    /// </summary>
    public bool IsRevealed { get { return isRevealed; } }
    private bool isVisible;
    /// <summary>
    /// 是否可见（看到最新数据）
    /// </summary>
    public bool IsVisible { get { return isVisible; } }
    public void DayIdle()
    {
        habitability = area.CalcurateHabitability();
        amount *= Random.Range(1.0f, habitability + 0.5f); //calcurate amount change referencing its habitability
        if (isVisible)
            UpdateVisibleData();
        HexCell cell= area.GetHexCell();
        cell.UpdateAnimals();
        UpdateHUD();
    }
    public void SetIfVisible(bool visible)
    {
        if (isVisible == visible)
            return;
        isVisible = visible;
        UpdateHUD();
    }
    private void UpdateHUD()
    {
        if (isRevealed)
        {
            if (isVisible)
            {
                area.habitatHealthImage.gameObject.SetActive(true);
                area.habitatMarkImage.sprite = habitatSprites[Level];
                if (habitability < 0.5f)
                    area.habitatMarkImage.color = Color.Lerp(Color.red, Color.white, habitability / 0.5f);
                else
                    area.habitatMarkImage.color = Color.Lerp(Color.white, Color.green, (habitability - 0.5f) / 0.5f);
                int min = animal.minHabitatPopulation(Level), max = animal.maxHabitatPopulation(Level);
                float fillAmount = (float)(amount - min) / (max - min);
                area.habitatHealthImage.fillAmount = fillAmount;
                area.habitatHealthImage.color = Color.Lerp(Color.red, Color.green, fillAmount);
            }
            else
            {
                area.habitatHealthImage.gameObject.SetActive(false);
                area.habitatMarkImage.sprite = unknownHabitatSprites[lastCheckedLevel];
            }
        }
    }
    public void Reveal()
    {
        if (isRevealed)
            return;
        isRevealed = true;
        UpdateVisibleData();
        area.habitatMarkImage.sprite = habitatSprites[Level];
        area.habitatMarkImage.gameObject.SetActive(true);
        NewsPanel.instance.AddNews(area.region.name + area.areaName + "地区 发现了新的栖息地 " + Level + "级", Resources.Load<Sprite>("UI/Icon/Habitat"));
    }
    public void UpdateVisibleData()
    {
        lastCheckedLevel = Level;
        lastCheckedHabitability = habitability;
    }
    public string TooltipDescription
    {
        get
        {
            return animal.animalName + "的" + Level + "级栖息地";
        }
    }
}
