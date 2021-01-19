using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventReportUI : MonoBehaviour
{
    public Event eventDetails;
    public Text EventDescription;
    public Text EventDescriptionAfterFinish;

    public GameObject SingleEventInfoPrefab;
    public GameObject EventInfoList;
    
    void Start()
    {
        InstantiateEventInfoList();
    }

    void Update()
    {
        EventDescription.text = eventDetails.description;
    }

    private void OnEnable ()
    {
        InstantiateEventInfoList();
    }

    public void InstantiateEventInfoList ()
    {
        for (int i = 0 ; i < eventDetails.includedInfos.Count ; i++)
        {
            GameObject clone = Instantiate(SingleEventInfoPrefab, EventInfoList.transform, false);
            clone.GetComponent<EventInfoUI>().eventInfo = eventDetails.includedInfos[i];
            InGameLog.AddLog(eventDetails.includedInfos[i].infoName);

        }
    }

    public void CloseWindow ()
    {
        Destroy(gameObject);
    }
}
