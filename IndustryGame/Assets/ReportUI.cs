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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void InstantiateSpecialistList ()
    //{
    //    for (int i = 0 ; i < specialists.Count ; i++)
    //    {
    //        GameObject clone;
    //        clone = Instantiate(SingleSpecialistPrefab, SpecialistList.transform, false);
    //        clone.GetComponent<SingleSpecialist>().specialist = specialists[i];
    //    }
    //}
}
