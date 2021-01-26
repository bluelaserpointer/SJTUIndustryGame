using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaDetailsHUD : MonoBehaviour
{
    public Text AreaName;

    void Update()
    {
        AreaName.text = OrthographicCamera.GetMousePointingArea().areaName;
    }
}
