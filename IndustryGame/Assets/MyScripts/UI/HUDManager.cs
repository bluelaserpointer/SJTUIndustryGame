using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager instance;
    public Text Date;
    public Text Money;

    public GameObject SpecialistWindow;
    public GameObject ReportWindow;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        Date.text = Timer.GetYear().ToString() + " " + Timer.GetMonth().ToString() + " " + Timer.GetDay().ToString() + " " + Timer.GetSeason().ToString();
        Money.text = Stage.GetLestMoney().ToString();
    }

    public static bool CheckOpenWindow ()
    {
        if (instance == null)
        {
            return false;
        }
        
        return instance.SpecialistWindow.activeSelf || instance.ReportWindow.activeSelf || GameObject.FindGameObjectWithTag("PopUpWindow") != null;
    }

    public void Pause ()
    {
        Timer.Pause();
    }

    public void Resume ()
    {
        Timer.Resume();
    }

    public void Speed1 ()
    {
        Timer.setTimeSpeed(1.0f);
    }

    public void Speed2 ()
    {
        Timer.setTimeSpeed(2.0f);
    }

    public void Speed3 ()
    {
        Timer.setTimeSpeed(3.0f);
    }


}
