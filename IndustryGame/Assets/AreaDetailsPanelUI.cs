using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaDetailsPanelUI : MonoBehaviour
{
    public Text AreaNameText;
    public Text EnvironmentTypeText;
    public Text RegionText;
    public Text DescriptionText;
    public GameObject FinshedActionsList;
    public GameObject UnfinshedEnabledActionsList;
    public GameObject ActionsDetailsPrefab;
    public GameObject FinishedBuildingsList;
    public GameObject BuildingsDetailsPrefab;

    public List<GameObject> FinishedActionsListPrefabs;
    public List<GameObject> UnfinshedEnabledActionsListPrefabs;
    public List<GameObject> FinishedBuildingsListPrefabs;

    void Start()
    {

    }
    void Update()
    {
        SetArea();
    }

    public void SetArea()
    {
        foreach (GameObject prefab in FinishedActionsListPrefabs)
        {
            Destroy(prefab);
        }
        foreach (GameObject prefab in UnfinshedEnabledActionsListPrefabs)
        {
            Destroy(prefab);
        }
        foreach (GameObject prefab in FinishedBuildingsListPrefabs)
        {
            Destroy(prefab);
        }

        FinishedActionsListPrefabs.Clear();
        UnfinshedEnabledActionsListPrefabs.Clear();
        FinishedBuildingsListPrefabs.Clear();


        AreaNameText.text = OrthographicCamera.GetMousePointingArea().areaName;
        EnvironmentTypeText.text = EnvironmentTypeDescription.GetEnvironmentTypeDescription(OrthographicCamera.GetMousePointingArea().environmentType);
        RegionText.text = OrthographicCamera.GetMousePointingArea().region.GetRegionId().ToString();
        DescriptionText.text = OrthographicCamera.GetMousePointingArea().description;

        foreach (AreaAction finishedAction in OrthographicCamera.GetMousePointingArea().GetFinishedActions())
        {
            GameObject clone = Instantiate(ActionsDetailsPrefab, FinshedActionsList.transform, false);
            clone.GetComponent<ActionsDetailsPrefab>().RefreshUI(finishedAction);
            FinishedActionsListPrefabs.Add(clone);
        }

        foreach (AreaAction unfinishedEnabledAction in OrthographicCamera.GetMousePointingArea().GetEnabledActions())
        {
            GameObject clone = Instantiate(ActionsDetailsPrefab, UnfinshedEnabledActionsList.transform, false);
            clone.GetComponent<ActionsDetailsPrefab>().RefreshUI(unfinishedEnabledAction);
            UnfinshedEnabledActionsListPrefabs.Add(clone);
        }

        int i = 0;

        foreach (Building building in OrthographicCamera.GetMousePointingArea().buildings)
        {
            GameObject clone = Instantiate(BuildingsDetailsPrefab, FinishedBuildingsList.transform, false);
            clone.GetComponent<BuildingsDetailsPrefab>().RefreshUI(building);
            FinishedBuildingsListPrefabs.Add(clone);
            i++;
        }
    }
}
