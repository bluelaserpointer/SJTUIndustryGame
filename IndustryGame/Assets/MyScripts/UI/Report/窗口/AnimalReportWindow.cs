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
        foreach (MainEvent mainEvent in Stage.GetRegions()[RegionSelection.value].GetRevealedEvents())
        {
            GameObject clone = GameObject.Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
            clone.GetComponent<SingleEventReport>().ShowSingleAnimal(mainEvent.concernedAnimal);
            AnimalReports.Add(clone);
            Debug.Log("Species: " + mainEvent.concernedAnimal.animalName);
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
