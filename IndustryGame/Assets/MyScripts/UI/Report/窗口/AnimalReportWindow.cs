using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalReportWindow : MonoBehaviour, BasicReportWindow
{
    [Header("生成小报告位置")]
    public GameObject EventReportList;
    [Header("小报告Prefab")]
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    [Header("Region的选择Dropdown")]
    public Dropdown RegionSelection;
    private List<GameObject> AnimalReports = new List<GameObject>();

    public void ClearList()
    {
        Helper.ClearList(AnimalReports);
    }

    public void GenerateList()
    {
        ClearList();
        List<Region> regions;
        if (RegionSelection.options[RegionSelection.value].text == "全部")
        {
            regions = Stage.GetRegions();
        }
        else
        {
            regions = new List<Region>();
            regions.Add(Stage.GetRegions()[RegionSelection.value]);
        }
        foreach (Region region in regions)
        {
            if(region.MainEvent != null && region.MainEvent.IsAppeared)
            {
                GameObject clone = Object.Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
                clone.GetComponent<SingleEventReport>().ShowSingleAnimal(region.MainEvent.concernedAnimal);
                AnimalReports.Add(clone);
            }
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
