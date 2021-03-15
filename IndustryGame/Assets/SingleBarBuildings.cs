using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleBarBuildings : MonoBehaviour
{
    [HideInInspector]
    public BuildingInfo buildingInfo;

    [Header("专家名字")]
    public Text buildingName;
    [Header("专家头像")]
    public Image buildingPhoto;

    public void RefreshUI(BuildingInfo buildingInfo)
    {
        this.buildingInfo = buildingInfo;
        buildingName.text = buildingInfo.name;
        buildingPhoto.sprite = buildingInfo.icon;
    }
}
