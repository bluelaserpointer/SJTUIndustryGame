using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float oneYear = 100000f;
    private float oneMonth;
    private float oneDay;
    private float currentTime = 0f;
    private float currentYear = 0f;

    // Start is called before the first frame update
    void Start()
    {
        oneMonth = oneYear / 4f;      
        oneDay = oneMonth / 30f;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
    }

    public SeasonType GetSeason()
    {
        int currentMonth = GetMonth();
        SeasonType currentSeason = SeasonType.Spring;
        switch (currentMonth)
        {
            case 1:
            case 2:
            case 12:
                currentSeason = SeasonType.Winter;
                break;
            case 3:
            case 4:
            case 5:
                currentSeason = SeasonType.Spring;
                break;
            case 6:
            case 7:
            case 8:
                currentSeason = SeasonType.Summer;
                break;
            case 9:
            case 10:
            case 11:
                currentSeason = SeasonType.Autumn;
                break;
        }

        return currentSeason;
    }

    public int GetDay()
    {
        return 0;
    }

    public int GetMonth()
    {
        int currentMonth = (int)((currentTime % oneYear) / oneMonth) + 1;
        return currentMonth;
    }

    public int GetYear()
    {
        int currentYear = (int)(currentTime / oneYear) + 1;
        return currentYear;
    }

}
