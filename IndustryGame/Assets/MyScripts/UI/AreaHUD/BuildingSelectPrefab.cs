using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectPrefab : MonoBehaviour
{
    private BuildingInfo buildingInfo;
    public Text buildingName;
    public Text buildingDescription;
    public Text MoneyCost;
    public Text TimeCost;

    public void RefreshUI (BuildingInfo buildingInfo)
    {
        this.buildingInfo = buildingInfo;
        buildingName.text = buildingInfo.buildingName;
        //buildingDescription.text = buildingInfo.description;
        //MoneyCost.text = buildingInfo.moneyCost.ToString();
        //buildingName.text = buildingInfo.timeCost.ToString();
    }

    public void StartBuild ()
    {
        OrthographicCamera.GetMousePointingArea().StartConstruction(buildingInfo);
        InGameLog.AddLog("Start building " + buildingInfo.buildingName);
    }

}
