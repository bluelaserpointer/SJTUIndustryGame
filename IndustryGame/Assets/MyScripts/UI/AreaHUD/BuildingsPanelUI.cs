using System.Collections.Generic;
using UnityEngine;

public class BuildingsPanelUI : MonoBehaviour
{
    public static BuildingsPanelUI instance;
    [HideInInspector]
    public List<GameObject> GeneratedPrefabs;
    public GameObject BuildingsGenerateList;
    public GameObject BuildingSelectPrefab;

    [HideInInspector]
    public BuildingInfo SelectedBuildingInfo;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GeneratePrefabs();
    }

    private void OnEnable()
    {
        GeneratePrefabs();
    }

    public void GeneratePrefabs()
    {
        if (GeneratedPrefabs != null)
        {
            foreach (GameObject prefab in GeneratedPrefabs)
            {
                prefab.GetComponent<BuildingSelectPrefab>().ClearObject();
            }
        }
        GeneratedPrefabs.Clear();

        Area area = OrthographicCamera.GetMousePointingArea();
        if (area != null)
        {
            foreach (BuildingInfo info in area.GetEnabledBuildings())
            {
                GameObject clone = Instantiate(BuildingSelectPrefab, BuildingsGenerateList.transform, false);
                clone.GetComponent<BuildingSelectPrefab>().RefreshUI(info);
                GeneratedPrefabs.Add(clone);
                //InGameLog.AddLog("Area: " + area.areaName + " Enabled Building: " + info.buildingName);
            }
        }
    }

    public void StartConstruction()
    {
        if (SelectedBuildingInfo != null)
        {
            OrthographicCamera.GetMousePointingArea().StartConstruction(SelectedBuildingInfo);
            GeneratePrefabs();
        }
    }

}
