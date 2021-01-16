public class Weather
{
    public enum WeatherType
    {
        Sunny,
        Rainy,
        Snowy
    }
    private WeatherType weatherType;
    private float temperature;

    public void judgeWeather() //TODO
    {
        switch(Timer.GetSeason())
        {

        }
        weatherType = WeatherType.Sunny;
        temperature = 25.0f;
    }
}
