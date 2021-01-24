using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleEventReport : MonoBehaviour
{
    public Event eventD;
    public GameObject EventReportUIPrefab;
    public GameObject InstantiateParent;
    public Image EventImage;
    public Text EventName;


    void Update()
    {
        EventName.text = eventD.eventName;
        //EventImage.sprite = eventD.
    }

    private void OnDisable ()
    {
        Destroy(gameObject);
    }

    public void DisplayEventReport ()
    {
        InstantiateParent = GameObject.FindGameObjectWithTag("ReportInstantiate");
        GameObject EventReportUI = Instantiate(EventReportUIPrefab, InstantiateParent.transform, false);
        EventReportUI.GetComponent<EventReportUI>().eventDetails = eventD;

        GameObject.FindGameObjectWithTag("ReportWindow").SetActive(false);


    }
}
