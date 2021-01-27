using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaDetailsPanelUI : MonoBehaviour
{
    public Text AreaNameText;
    public Text EnvironmentTypeText;
    public Text RegionText;
    public Text DescriptionText;
    void Start()
    {

    }
    void Update()
    {
        SetArea();
    }

    public void SetArea()
    {
        AreaNameText.text = OrthographicCamera.GetMousePointingArea().areaName;
        EnvironmentTypeText.text = EnvironmentTypeDescription.GetEnvironmentTypeDescription(OrthographicCamera.GetMousePointingArea().environmentType);
        RegionText.text = OrthographicCamera.GetMousePointingArea().region.GetRegionId().ToString();
        DescriptionText.text = OrthographicCamera.GetMousePointingArea().description;
    }
}
