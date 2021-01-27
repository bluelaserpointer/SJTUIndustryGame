using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventReportUI : MonoBehaviour
{
    public MainEvent eventDetails;
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
        for (int i = 0 ; i < eventDetails.GetRevealedInfos().Count ; i++)
        {
            GameObject clone = Instantiate(SingleEventInfoPrefab, EventInfoList.transform, false);
            clone.GetComponent<EventInfoUI>().eventInfo = eventDetails.GetRevealedInfos()[i];
            InGameLog.AddLog(eventDetails.GetRevealedInfos()[i].infoName);

        }
    }

    public void CloseWindow ()
    {
        Destroy(gameObject);
    }
}
