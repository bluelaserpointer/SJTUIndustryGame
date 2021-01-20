using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportUI : MonoBehaviour
{
    public GameObject EventReportList;
    public GameObject AnimalReportList;
    public GameObject EnvironmentReportList;
    public GameObject SingleEventReportPrefab;      //报告Prefab，三个都应该是一样的
    private List<Event> events;
    private List<EventInfo> eventinfos;
    
    void Start()
    {
        events = Stage.GetEvents();
        InstantiateEventList();
    }

    private void OnEnable ()
    {
        InstantiateEventList();
        InGameLog.AddLog("In SpecialistUI OnEnable");

    }

    void Update()
    {
        events = Stage.GetEvents();
    }


    void InstantiateEventList ()
    {
        for (int i = 0 ; i < events.Count ; i++)
        {
            GameObject clone;
            clone = Instantiate(SingleEventReportPrefab, EventReportList.transform, false);
            clone.GetComponent<SingleEventReport>().eventD = events[i];

        }
    }
}
