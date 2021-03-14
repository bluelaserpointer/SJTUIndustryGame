using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventReportWindow : MonoBehaviour, BasicReportWindow
{
    [Header("生成小报告位置")]
    public GameObject EventReportList;
    [Header("小报告Prefab")]
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    [Header("Region的选择Dropdown")]
    public Dropdown RegionSelection;
    private List<GameObject> EventReports = new List<GameObject>();

    public void ClearList()
    {
        Helper.ClearList(EventReports);
    }

    public void GenerateList()
    {
        ClearList();
        if (RegionSelection.options[RegionSelection.value].text == "全部")
        {
            foreach (Region region in Stage.GetRegions())
            {
                foreach (MainEvent mainEvent in region.GetRevealedEvents())
                {
                    GameObject clone = GameObject.Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
                    clone.GetComponent<SingleEventReport>().ShowSingleEvent(mainEvent);
                    EventReports.Add(clone);
                }
            }
        }
        else
        {
            foreach (MainEvent mainEvent in Stage.GetRegions()[RegionSelection.value].GetRevealedEvents())
            {
                GameObject clone = GameObject.Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
                clone.GetComponent<SingleEventReport>().ShowSingleEvent(mainEvent);
                EventReports.Add(clone);
            }
        }
    }

    void Start()
    {
        Helper.RefreshRegionDropdown(RegionSelection);
        GenerateList();
    }

    void Update()
    {
        Helper.RefreshRegionDropdown(RegionSelection);
    }

}
