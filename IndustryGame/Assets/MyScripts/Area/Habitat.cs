using UnityEngine;

public class Habitat
{
    private static readonly Sprite[] sprites = new Sprite[6];
    static Habitat()
    {
        for(int i = 0; i <= 5; ++i)
        {
            sprites[i] = Resources.Load<Sprite>("UI/Area/Habitat" + i);
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
    private float habitability = 0.5f;
    public float Habitability { get { return habitability; } }
    private bool isRevealed;
    /// <summary>
    /// 是否已发现
    /// </summary>
    public bool IsRevealed { get { return isRevealed; } }
    public void DayIdle()
    {
        //TODO: calcurate habitability depends on nearby environment problems
        habitability = 0.5f;
        foreach (Area eachArea in area.GetNeighborAreas()) {
            foreach(EnvironmentStatFactor factor in eachArea.environmentStatFactors)
            {
                habitability += factor.ReceiveAffect(1);
            }
        }
        foreach (EnvironmentStatFactor factor in area.environmentStatFactors)
        {
            habitability += factor.ReceiveAffect(0);
        }
        //calcurate amount decrease depends on habitability
        amount *= Random.Range(1.0f, habitability + 0.5f);
        //change appearance of habitat mark
        if (isRevealed)
        {
            area.habitatMarkImage.sprite = sprites[Level];
            if (habitability < 0.5f)
                area.habitatMarkImage.color = Color.Lerp(Color.red, Color.white, habitability / 0.5f);
            else
                area.habitatMarkImage.color = Color.Lerp(Color.white, Color.green, (habitability - 0.5f) / 0.5f);
        }
    }
    public void Reveal()
    {
        if (isRevealed)
            return;
        isRevealed = true;
        area.habitatMarkImage.sprite = sprites[Level];
        area.habitatMarkImage.gameObject.SetActive(true);
        NewsPanel.instance.AddNews(area.region.name + area.areaName + "地区 发现了新的栖息地 " + Level + "级", Resources.Load<Sprite>("UI/Icon/Habitat"));
    }
}
