using System.Collections.Generic;
using UnityEngine;

public class BuildingsPanelUI:MonoBehaviour
{
    public List<GameObject> GeneratedPrefabs;
    public GameObject BuildingsGenerateList;
    public GameObject BuildingSelectPrefab;

    private void Start ()
    {
        GeneratePrefabs();
    }

    private void OnEnable ()
    {
        GeneratePrefabs();
    }

    public void GeneratePrefabs ()
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
        if(area != null)
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



}
