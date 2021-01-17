public class Weather
{
    public enum WeatherType
    {
        Sunny,
        Rainy,
        Snowy
    }
    private WeatherType weatherType;

    // 空气温度
    private float temperature;
    private float[] temperatureHighStandard = {7.6f, 8.7f, 12.6f, 18.5f, 23.2f, 27.8f, 31.8f, 31.6f, 27.4f, 22.4f, 16.8f, 10.7f};
    private float[] temperatureLowStandard = {0.3f, 1.1f, 4.9f, 10.4f, 15.3f, 20.1f, 24.7f, 24.7f, 20.5f, 14.3f, 8.6f, 2.7f};

    // 空气湿度
    private float humidity;
    private float[] humidityStandard = {74f, 68f, 80f, 75f, 76f, 87f, 85f, 84f, 81f, 76f, 73f, 66}; 

    private float groundWater; // 地面储水量
    private float skyWater; // 云朵储水量

    private float waterFallLimit = 70f;

    public Weather(){
        temperature = 25.0f;
        humidity = 77.0f;
        groundWater = 75f;
        skyWater = 25f;
    }

    public Weather(float t, float h){
        temperature = t;
        humidity = h;
    }

    public void judgeWeather() //TODO
    {
        int day = Timer.GetDay();
        int month = Timer.GetMonth();
        float ratio = (float)day / (float)(Timer.oneDay * Timer.dayInMonth);
        SeasonType season = Timer.GetSeason();

        handleTemperature(month, day, ratio);
        handleHumidity(month, day, ratio);
        
        // 根据 WeatherType 设置地面云朵储水量
        switch (weatherType)
        {
            // 降雨、降雪
            case WeatherType.Rainy:
            case WeatherType.Snowy:
                skyWater -= 20;
                groundWater += 20;
                break;
            
            // 蒸发
            case WeatherType.Sunny:
                skyWater += 10;
                groundWater -= 10;
                break;
        }

        // 根据 季节、云朵储水量设置天气
        if(skyWater < 30f || season == SeasonType.Autumn)
            weatherType = WeatherType.Sunny;
        else
            switch(season)
            {
                case SeasonType.Spring:
                case SeasonType.Summer:
                    // 云朵储水量大于90
                    if(skyWater > waterFallLimit)
                        weatherType = WeatherType.Rainy;
                break;

                case SeasonType.Winter:
                    // 云朵储水量大于90
                    if(skyWater > waterFallLimit)
                    {
                        // 初冬降雨，深冬降雪
                        if(ratio > 0.5f)
                            weatherType = WeatherType.Snowy;
                        else
                            weatherType = WeatherType.Rainy;
                    }
                break;
            }
    }

    public void handleTemperature(int month, int day, float ratio)
    {
        int currMonth = month - 1;
        int nextMonth = currMonth < 11 ? currMonth + 1 : 0;
        float currHigh = temperatureHighStandard[currMonth];
        float currLow = temperatureLowStandard[currMonth];
        float nextHigh = temperatureHighStandard[nextMonth];
        float nextLow = temperatureLowStandard[nextMonth];
        
        float currAvg = (currHigh + currLow) / 2f; // TODO: Randomize
        float nextAvg = (nextHigh + nextLow) / 2f; // TODO: Randomize

        float diffAvg = nextAvg - currAvg;

        temperature = currAvg + ratio * diffAvg;
    }

    public void handleHumidity(int month, int day, float ratio)
    {
        int currMonth = month - 1;
        int nextMonth = currMonth < 11 ? currMonth + 1 : 0;
        float curr = humidityStandard[currMonth];
        float next = humidityStandard[nextMonth];

        float diff = next - curr;

        humidity = curr + ratio * diff;
    }
}
