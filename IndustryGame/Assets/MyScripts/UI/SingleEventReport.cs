using UnityEngine;
using UnityEngine.UI;

public class SingleEventReport : MonoBehaviour
{
    public MainEvent eventD;
    public GameObject EventReportUIPrefab;
    public GameObject InstantiateParent;
    public Image EventImage;
    public Text EventName;


    void Update()
    {
        if(eventD != null)
            EventName.text = eventD.name;
        //EventImage.sprite = eventD.
    }

    private void OnDisable ()
    {
        Destroy(gameObject);
    }

    public void DisplayEventReport ()
    {
        if (eventD == null)
            return;
        InstantiateParent = GameObject.FindGameObjectWithTag("ReportInstantiate");
        GameObject EventReportUI = Instantiate(EventReportUIPrefab, InstantiateParent.transform, false);
        EventReportUI.GetComponent<EventReportUI>().eventDetails = eventD;

        GameObject.FindGameObjectWithTag("ReportWindow").SetActive(false);


    }
}
