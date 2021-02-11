using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EventReportUI : MonoBehaviour
{
    [HideInInspector]
    public MainEvent eventDetails;
    [Header("显示报告描述的Text")]
    public Text EventDescription;
    [Header("显示报告结束后描述的Text")]
    public Text EventDescriptionAfterFinish;

    [Header("单个报告的Prefab")]
    public GameObject SingleEventInfoPrefab;
    [Header("生成单个报告的位置")]
    public GameObject EventInfoList;

    [Header("单个可执行措施的Prefab")]
    public GameObject EnabledActionPrefab;
    [Header("生成可执行措施的位置")]
    public GameObject EnabledActionsList;



    private List<GameObject> EventInfoPrefabs = new List<GameObject>();
    private List<GameObject> GeneratedActionPrefabs = new List<GameObject>();

    // 如果有点击事件发生则不能使用Update
    void Update()
    {
        RefreshUI();
    }


    public void RefreshUI()
    {
        if (eventDetails == null)
        {
            InGameLog.AddLog("Error: Event is not assigned");
            return;
        }
        Helper.ClearList(EventInfoPrefabs);
        Helper.ClearList(GeneratedActionPrefabs);

        EventDescription.text = eventDetails.description;
        EventDescriptionAfterFinish.text = eventDetails.descriptionAfterFinish;


        foreach (EventStage eventInfo in eventDetails.GetRevealedEventStages())
        {
            GameObject clone = Instantiate(SingleEventInfoPrefab, EventInfoList.transform, false);
            clone.GetComponent<EventInfoUI>().Generate(eventInfo);
            // InGameLog.AddLog(areaAction.actionName);
            EventInfoPrefabs.Add(clone);
        }

        foreach (AreaAction areaAction in eventDetails.includedAreaActions)
        {
            GameObject clone = Instantiate(EnabledActionPrefab, EnabledActionsList.transform, false);
            clone.GetComponent<EnabledActionPrefab>().Generate(areaAction);
            InGameLog.AddLog(areaAction.actionName);
            GeneratedActionPrefabs.Add(clone);
        }
    }

}
