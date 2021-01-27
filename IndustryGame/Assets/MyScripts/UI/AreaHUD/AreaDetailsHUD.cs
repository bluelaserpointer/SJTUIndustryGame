using UnityEngine;
using UnityEngine.UI;

public class AreaDetailsHUD : MonoBehaviour
{
    public Text AreaName;

    void Update()
    {
        Area area = OrthographicCamera.GetMousePointingArea();
        if(area != null)
            AreaName.text = area.areaName;
    }
}
