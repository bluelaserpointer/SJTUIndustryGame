using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentReportWindow : MonoBehaviour, BasicReportWindow
{
    [Header("生成小报告位置")]
    public GameObject EnvironmentReportList;
    [Header("小报告Prefab")]
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    [Header("Region的选择Dropdown")]
    public Dropdown RegionSelection;

    private List<GameObject> EnvironmentReports = new List<GameObject>();


    public void ClearList()
    {
        Helper.ClearList(EnvironmentReports);
    }

    public void GenerateList()
    {
        ClearList();
        foreach (EventStage eventInfo in Stage.GetRegions()[RegionSelection.value].GetEventInfosRelatedToEnvironment())
        {
            GameObject clone = GameObject.Instantiate(SingleEventReportPrefab, EnvironmentReportList.transform, false);
            clone.GetComponent<SingleEventReport>().ShowSingleEnvironment(eventInfo);
            EnvironmentReports.Add(clone);
        }
    }

    // 有点击事件发生所以要手动调用GenerateList来更新列表，而不能用Update实时更新
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
