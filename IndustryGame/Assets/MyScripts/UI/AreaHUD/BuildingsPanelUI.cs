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

    private void GeneratePrefabs ()
    {
        GeneratedPrefabs.Clear();
        Area area = OrthographicCamera.GetMousePointingArea();
        if(area != null)
        {
            for (int i = 0; i < area.GetEnabledBuildings().Count; i++)
            {
                GameObject clone = Instantiate(BuildingSelectPrefab, BuildingsGenerateList.transform, false);
                clone.GetComponent<BuildingSelectPrefab>().RefreshUI(OrthographicCamera.GetMousePointingArea().GetEnabledBuildings()[i]);
                GeneratedPrefabs.Add(clone);
            }
        }
    }



}
