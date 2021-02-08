using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBarButton : MonoBehaviour
{
    [Header("此按钮对应哪一种窗口")]
    public ReportSelectionWindowType buttonType;
    // Start is called before the first frame update
    public void ButtonOnClickEvent()
    {
        switch (buttonType)
        {
            case ReportSelectionWindowType.EventType:
                ReportUI.GenerateEventWindow();
                break;
            case ReportSelectionWindowType.AnimalType:
                ReportUI.GenerateAnimalWindow();
                break;
            case ReportSelectionWindowType.EnvironmentType:
                ReportUI.GenerateEnvironmentWindow();
                break;

        }
    }
}
