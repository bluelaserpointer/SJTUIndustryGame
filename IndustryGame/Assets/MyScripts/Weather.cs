using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    // Start is called before the first frame update
    public enum WeatherType
    {
        Sunny,
        Rainy,
        Snowy
    }
    private float period;

    // private Timer global;

    void Start()
    {
        // global = GameObject.FindGameObjectWithTag("Global").GetComponent<Global>();
        // period = global.;
    }

    // Update is called once per frame
    void Update()
    {
        
        periodCountDown();
    }

    private void periodCountDown()
    {
        period -= Time.deltaTime;

        if(period <= 0f)
        {
            weatherChange();
        }
    }

    // TODO: Change to another weather
    private void weatherChange()
    {

    }
}
