using UnityEngine;

public class Timer
{
    public static float secondsOneDay = 1f; //更改游戏速度 (1 sec = 1 game day)
    public static float secondsOneMonth = secondsOneDay * 30;
    public static float secondsOneYear = secondsOneMonth * 12;
    public static float timeSpeed = 1.0f;
    private static float currentTime = 0f;
    private static int currentYear, currentMonth, currentDay;

    // Update is called once per frame
    public static void idle()
    {
        currentTime += Time.deltaTime * timeSpeed;
        currentDay = (int)((currentTime % secondsOneYear) % secondsOneMonth) + 1;
        currentMonth = (int)((currentTime % secondsOneYear) / secondsOneMonth) + 1;
        currentYear = (int)(currentTime / secondsOneYear) + 2021;
    }

    public static SeasonType GetSeason()
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

    public static int GetDay()
    {
        return currentDay;
    }

    public static int GetMonth()
    {
        return currentMonth;
    }

    public static int GetYear()
    {
        return currentYear;
    }
}
