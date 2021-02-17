using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class BuildingSelectPrefab : MonoBehaviour
{
    private BuildingInfo buildingInfo;
    public Text buildingName;
    public Text buildingDescription;
    public Text MoneyCost;
    public Text TimeCost;
    private Button button;
    public Image SelectImage;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(SelectBuilding);
        // button.onClick.AddListener(() => gameObject.GetComponentInParent<FilterPanelFocusHelper>().SelectImage(SelectImage, SelectText));
        // button.onClick.AddListener(() => gameObject.GetComponentInParent<FilterPanelFocusHelper>().SelectImage(SelectImage, MoneyText));

        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Deselect;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(DeselectBuildingInfo);

        trigger.triggers.Add(entry);
    }

    public void RefreshUI(BuildingInfo buildingInfo)
    {
        this.buildingInfo = buildingInfo;
        buildingName.text = buildingInfo.buildingName;
        //buildingDescription.text = buildingInfo.description;
        MoneyCost.text = buildingInfo.moneyCost.ToString() + "$";
        //buildingName.text = buildingInfo.timeCost.ToString();
    }

    public void SelectBuilding()
    {
        GameObject.FindGameObjectWithTag("BuildingGeneratePanel").GetComponent<BuildingsPanelUI>().SelectedBuildingInfo = buildingInfo;

        OrthographicCamera.GetMousePointingArea().StartConstruction(buildingInfo);
        GameObject.FindGameObjectWithTag("BuildingGeneratePanel").GetComponent<BuildingsPanelUI>().GeneratePrefabs();
    }

    public void DeselectBuildingInfo(BaseEventData pointData)
    {
        GameObject.FindGameObjectWithTag("BuildingGeneratePanel").GetComponent<BuildingsPanelUI>().SelectedBuildingInfo = null;
    }

    public void ClearObject()
    {
        Destroy(gameObject);
    }

}
