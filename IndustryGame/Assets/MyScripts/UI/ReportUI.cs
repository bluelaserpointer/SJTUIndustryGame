using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportUI : MonoBehaviour
{
    public GameObject EventReportList;
    public GameObject AnimalReportList;
    public GameObject EnvironmentReportList;
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    private List<EventStageSO> eventinfos;
    
    void Start()
    {
    }

    private void OnEnable ()
    {
        InstantiateEventList();
        InGameLog.AddLog("In SpecialistUI OnEnable");
    }

    void Update()
    {
    }


    void InstantiateEventList ()
    {
        foreach (MainEvent mainEvent in Stage.GetRevealedEvents())
        {
            GameObject clone;
            clone = Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
            clone.GetComponent<SingleEventReport>().eventD = mainEvent;

        }
    }
}
