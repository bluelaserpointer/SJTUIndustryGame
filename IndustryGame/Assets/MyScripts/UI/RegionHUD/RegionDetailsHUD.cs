using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionDetailsHUD : MonoBehaviour
{
    public static RegionDetailsHUD instance;

    [Header("显示地域名字的Text")]
    public Text RegionName;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        // SetMousePointingArea();
    }

    public static void SetMousePointingRegion()
    {
        if (OrthographicCamera.GetMousePointingRegion() != null && OrthographicCamera.GetMousePointingArea() == null)
        {
            instance.RegionName.text = OrthographicCamera.GetMousePointingRegion().name;
        }
    }
}
