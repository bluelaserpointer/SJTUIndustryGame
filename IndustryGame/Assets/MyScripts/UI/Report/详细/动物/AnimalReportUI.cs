using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnimalReportUI : MonoBehaviour
{
    [HideInInspector]
    public Animal animal;
    [Header("动物名字的Text")]
    public Text animalNameText;
    [Header("动物描述的Text")]
    public Text animalDescriptionText;
    [Header("动物最适合的环境的Text")]
    public Text animalBestEnvironmentText;
    [Header("动物自然生长速度的Text")]
    public Text animalReproductionSpeedText;
    [Header("动物所需能量的Text")]
    public Text animalEnergyNeedsText;
    [Header("动物提供能量的Text")]
    public Text animalEnergyAsFoodText;
    [Header("动物最低气温的Slider")]
    public Slider animalMinTemperatureSlider;
    [Header("动物最高气温的Text")]
    public Slider animalMaxTemparatureSlider;
    [Header("动物迁徙数量上限")]
    public Text animalMigrateLimitText;
    [Header("动物危险级别划分数")]
    public Text animalMostDangerLimitText;
    [Header("最低气温数字显示")]
    public Text animalMinTemperatureText;
    [Header("最高气温数字显示")]
    public Text animalMaxTemperatureText;



    [Header("生成Foods列表的位置")]
    public GameObject FoodsListPosition;
    [Header("单个显示的食物的 UI Prefab")]
    public GameObject SingleFoodsPrefab;
    [Header("生成Environment Preference列表的位置")]
    public GameObject EnvironmentPreferencePosition;
    [Header("单个显示的Environment Preference的 UI Prefab")]
    public GameObject SingleEnvironmentPreferencePrefab;

    // [Header("生成DangerLimits列表的位置")]
    // public GameObject DangerLimitsListPosition;
    // [Header("单个显示的DangerLimits的 UI Prefab")]
    // public GameObject SingleDangerLimitsPrefab;


    [Header("生成模型的位置")]
    public GameObject ModelGeneratePosition;


    private List<GameObject> GeneratedSingleFoodsPrefabs = new List<GameObject>();
    private List<GameObject> GeneratedSingleEnvironmentPreferencePrefabs = new List<GameObject>();
    // private List<GameObject> GeneratedSingleDangerLimitsPrefab = new List<GameObject>();


    // 如果有点击事件发生则不能使用Update
    void Update()
    {
        RefreshUI();
    }


    public void RefreshUI()
    {
        if (animal == null)
        {
            InGameLog.AddLog("Error: Animal is not assigned");
            return;
        }
        Helper.ClearList(GeneratedSingleFoodsPrefabs);
        Helper.ClearList(GeneratedSingleEnvironmentPreferencePrefabs);

        animalNameText.text = animal.animalName;
        animalDescriptionText.text = animal.description;
        animalBestEnvironmentText.text = "最适合环境: " + animal.bestEnvironmentType.ToString();
        animalReproductionSpeedText.text = "自然生长速度: " + animal.reproductionSpeed.ToString();
        animalEnergyNeedsText.text = "动物所需能量: " + animal.energyNeeds.ToString();
        animalEnergyAsFoodText.text = "动物提供能量: " + animal.energyAsFood.ToString();
        animalMigrateLimitText.text = "迁徙数量上限: " + animal.migrateLimit.ToString();
        animalMostDangerLimitText.text = "危险级别划分数: " + animal.mostDangerLimit.ToString();
        animalMinTemperatureSlider.value = ((float)animal.minTempreture + 30.0f) / 80.0f;     //TODO: Change this to the proper range of temparature
        animalMaxTemparatureSlider.value = ((float)animal.maxTempreture + 30.0f) / 80.0f;
        animalMinTemperatureText.text = animal.minTempreture.ToString() + "℃";
        animalMaxTemperatureText.text = animal.maxTempreture.ToString() + "℃";

        foreach (Animal food in animal.foods)
        {
            GameObject clone = Instantiate(SingleFoodsPrefab, FoodsListPosition.transform, false);
            clone.GetComponent<SingleFoodPrefab>().RefreshUI(food);
            // InGameLog.AddLog(eventDetails.GetRevealedInfos()[i].infoName);
            GeneratedSingleFoodsPrefabs.Add(clone);
        }

        foreach (Animal.EnvironmentPreference environmentPreference in animal.environmentPreferences)
        {
            GameObject clone = Instantiate(SingleEnvironmentPreferencePrefab, EnvironmentPreferencePosition.transform, false);
            clone.GetComponent<SingleEnvironmentPrefab>().RefreshUI(environmentPreference);
            // clone.GetComponent<EnabledActionPrefab>().Generate(areaAction);
            // InGameLog.AddLog(eventDetails.GetRevealedInfos()[i].infoName);
            GeneratedSingleEnvironmentPreferencePrefabs.Add(clone);
        }
    }
}
