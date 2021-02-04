using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalReportWindow : MonoBehaviour, BasicReportWindow
{
    [Header("生成小报告位置")]
    public GameObject EventReportList;
    [Header("小报告Prefab")]
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    private List<GameObject> AnimalReports = new List<GameObject>();

    public void ClearList()
    {
        Helper.ClearList(AnimalReports);
    }

    public void GenerateList()
    {
        ClearList();
        foreach (Animal animal in Stage.GetSpecies())
        {
            GameObject clone = GameObject.Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
            clone.GetComponent<SingleEventReport>().ShowSingleAnimal(animal);
            AnimalReports.Add(clone);
        }
    }

    // 有点击事件发生所以要手动调用GenerateList来更新列表，而不能用Update实时更新
    void Start()
    {
        GenerateList();
    }
}
