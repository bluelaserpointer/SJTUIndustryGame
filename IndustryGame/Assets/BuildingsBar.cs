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

    private List<GameObject> GeneratedBuildingss = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        RefreshList();
    }

    void Update()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        Helper.ClearList(GeneratedBuildingss);
        if (OrthographicCamera.GetMousePointingArea() == null || OrthographicCamera.GetMousePointingArea().GetEnabledBuildings() == null) return;
        foreach (BuildingInfo building in OrthographicCamera.GetMousePointingArea().GetEnabledBuildings())
        {
            Debug.Log(building.buildingName);
            GameObject clone = Instantiate(BuildingsImagePrefab, GenerateBuildingsImagePosition.transform, false);
            clone.GetComponent<SingleBarBuildings>().RefreshUI(building);
            GeneratedBuildingss.Add(clone);
        }
    }
}
