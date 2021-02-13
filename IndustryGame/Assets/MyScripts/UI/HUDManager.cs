﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager instance;
    public Text Date;
    public Text Money;
    public Text Contributions;
    public Text Opinion;
    public Text Reputation;

    public Color SelectedColor;
    public Color NormalColor;


    private Image SelectedButton;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        Date.text = Timer.GetYear().ToString() + " " + Timer.GetMonth().ToString() + " " + Timer.GetDay().ToString() + " " + Timer.GetSeason().ToString();
        Money.text = Stage.GetResourceValue(ResourceType.money).ToString();
        Opinion.text = Stage.GetResourceValue(ResourceType.opinion).ToString();
        Contributions.text = Stage.GetResourceValue(ResourceType.contribution).ToString();
        Reputation.text = Stage.GetResourceValue(ResourceType.reputation).ToString();
    }



    public void Pause(Image PauseButton)
    {
        Timer.Pause();
        UpdateButtons(PauseButton);
    }

    public void Resume()
    {
        Timer.Resume();
    }

    public void Speed1(Image SpeedOneButton)
    {
        Timer.setTimeSpeed(1.0f);
        UpdateButtons(SpeedOneButton);
    }

    public void Speed2(Image SpeedTwoButton)
    {
        Timer.setTimeSpeed(2.0f);
        UpdateButtons(SpeedTwoButton);
    }

    public void Speed3(Image SpeedThreeButton)
    {
        Timer.setTimeSpeed(3.0f);
        UpdateButtons(SpeedThreeButton);
    }

    private void UpdateButtons(Image SelectButton)
    {
        if (SelectedButton != null)
        {
            SelectedButton.color = NormalColor;
        }
        SelectedButton = SelectButton;
        SelectedButton.color = SelectedColor;
    }


}
