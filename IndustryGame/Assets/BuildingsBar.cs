using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsBar : MonoBehaviour
{
    public static BuildingsBar instance;

    [Header("生成建筑图片位置")]
    public GameObject GenerateBuildingsImagePosition;
    [Header("生成的建筑Prefab")]
    public GameObject BuildingsImagePrefab;

    [Header("暂无界面")]
    public GameObject EmptyPanel;

    private List<GameObject> GeneratedBuildingss = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        Helper.ClearList(GeneratedBuildingss);

        EmptyPanel.SetActive(true);
        while (Stage.getAreas() == null) { continue; }

        EmptyPanel.SetActive(false);
        foreach (BuildingInfo building in Stage.GetEnabledBuildings(Stage.getAreas()[0])[0].AllTypes)
        {
            //Debug.Log(building.buildingName);
            GameObject clone = Instantiate(BuildingsImagePrefab, GenerateBuildingsImagePosition.transform, false);
            clone.GetComponent<SingleBarBuildings>().RefreshUI(building);
            GeneratedBuildingss.Add(clone);
        }
    }
}
