using UnityEngine;
using UnityEngine.UI;

public class AreaDetailsHUD : MonoBehaviour
{
    [Header("显示地区名字的Text")]
    public Text AreaName;
    [Header("显示地域名字的Text")]
    public Text RegionName;

    void Update()
    {
        // SetMousePointingArea();
    }

    public void SetMousePointingArea()
    {
        if (OrthographicCamera.GetMousePointingArea() != null)
        {
            AreaName.text = OrthographicCamera.GetMousePointingArea().areaName;
            RegionName.text = OrthographicCamera.GetMousePointingArea().region.name;
        }
    }
}
