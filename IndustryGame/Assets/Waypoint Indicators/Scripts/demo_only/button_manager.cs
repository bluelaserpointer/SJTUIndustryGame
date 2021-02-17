using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_manager : MonoBehaviour
{
    #region Load Levels
    public void ResetToTitleScreen()
    {
        event_manager.onResetToTitleScreen();
    }

    public void LoadDayScene()
    {
        event_manager.onLoadScene(1);
    }

    public void LoadNightScene()
    {
        event_manager.onLoadScene(2);
    }

    public void CloseInstructionWindow()
    {
        event_manager.onCloseOptionScreenWithX();
    }

    #endregion


}
