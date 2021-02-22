using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterPanel : MonoBehaviour
{
    public static FilterPanel instance;
    [Header("生成Region选项的位置")]
    public GameObject RegionSelectionGeneratePosition;
    [Header("Region选项的Prefab")]
    public GameObject RegionSelectionPrefab;
    private List<GameObject> GeneratedRegionSelections = new List<GameObject>();
    [Header("生成Event选项的位置")]
    public GameObject EventSelectionGeneratePosition;
    [Header("Event选项的Prefab")]
    public GameObject EventSelectionPrefab;
    private List<GameObject> GeneratedEventSelections = new List<GameObject>();
    public FilterPanelFocusHelper focusHelperForRegion;
    public FilterPanelEventFocusHelper focusHelperForEvent;

    public GameObject AnimalSelectionPanel;

    private GameObject GeneratedAnimalSelectionPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        RefreshRegions();
        RefreshEventsDueToRegionSelection(instance.GeneratedRegionSelections[0].GetComponent<FilterPanelRegionSelect>().region);

    }

    void Update()
    {
    }

    public static void RefreshRegions()
    {
        Helper.ClearList(instance.GeneratedRegionSelections);

        foreach (Region region in Stage.GetRegions())
        {
            if (region.regionId == -1)
            {
                continue;
            }
            GameObject clone = Instantiate(instance.RegionSelectionPrefab, instance.RegionSelectionGeneratePosition.transform, false);
            clone.GetComponent<FilterPanelRegionSelect>().region = region;
            instance.GeneratedRegionSelections.Add(clone);
        }
    }

    public static void RefreshEventsDueToRegionSelection(Region region)
    {
        // Debug.Log("Refreshing Events for region: " + region.name);
        Destroy(instance.GeneratedAnimalSelectionPanel);
        Stage.ShowAnimalNumberPop(null);

        Helper.ClearList(instance.GeneratedEventSelections);
        foreach (MainEvent mainEvent in region.GetRevealedEvents())
        {
            // Debug.Log("Detected main event: " + mainEvent.name + " for region: " + region.name);
            GameObject clone = Instantiate(instance.EventSelectionPrefab, instance.EventSelectionGeneratePosition.transform, false);
            clone.GetComponent<FilterPanelEventSelect>().mainEvent = mainEvent;
            instance.GeneratedEventSelections.Add(clone);
        }
        // FilterPanelRegionSelect StartSelectedRegion = instance.GeneratedRegionSelections.f .GetComponent<FilterPanelRegionSelect>();
        foreach (GameObject gameObject in instance.GeneratedRegionSelections)
        {
            if (gameObject.GetComponent<FilterPanelRegionSelect>().region == region)
            {
                instance.focusHelperForRegion.SelectImage(gameObject.GetComponent<FilterPanelRegionSelect>().BackgroundImage, gameObject.GetComponent<FilterPanelRegionSelect>().RegionName);
            }
        }


    }

    public static void GenerateAnimalSelectionPanel(MainEvent mainEvent)
    {
        if (instance.GeneratedAnimalSelectionPanel != null)
        {
            Destroy(instance.GeneratedAnimalSelectionPanel);
        }
        instance.GeneratedAnimalSelectionPanel = Instantiate(instance.AnimalSelectionPanel, instance.transform, false);
        instance.GeneratedAnimalSelectionPanel.GetComponent<FilterPanelAnimalPanel>().mainEvent = mainEvent;
        instance.GeneratedAnimalSelectionPanel.GetComponent<FilterPanelAnimalPanel>().RefreshUI();
    }
}
