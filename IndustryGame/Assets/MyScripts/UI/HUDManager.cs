using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;
    public Text Date;
    public Text Money;
    public Text Contributions;
    public Text Opinion;
    public Text Reputation;

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



    public void Pause()
    {
        Timer.Pause();
    }

    public void Resume()
    {
        Timer.Resume();
    }

    public void Speed1()
    {
        Timer.setTimeSpeed(1.0f);
    }

    public void Speed2()
    {
        Timer.setTimeSpeed(2.0f);
    }

    public void Speed3()
    {
        Timer.setTimeSpeed(3.0f);
    }

}
