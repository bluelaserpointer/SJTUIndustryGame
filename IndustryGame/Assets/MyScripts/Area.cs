﻿using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Area : MonoBehaviour
{
    [SerializeField]
    private GameObject rainFX, snowFX, markSpecialist, markBasement;
    public string areaName;
    [TextArea]
    public string description;
    public EnvironmentType environmentType;

    public List<Building> buildings;
    [Serializable]
    public struct Stat
    {
        public AreaStat statType;
        public double value;
    }
    public List<Stat> stat;

    List<Area> neighbors = new List<Area>();
    [SerializeField]
    private List<Action> enabledActions;
    private struct AmountChange
    {
        public int old;
        private int current;
        public int change;
        public AmountChange(int initialValue) { old = current = initialValue; change = 0; }
        public void addCurrent(int value) { current += value; }
        public void recordChange() {
            if (current < 0)
                current = 0;
            change = current - old;
            old = current;
        }
    }
    private Dictionary<Animal, AmountChange> animalAmounts = new Dictionary<Animal, AmountChange>();
    private LinkedList<Dictionary<Animal, AmountChange>> animalAmountsRecords = new LinkedList<Dictionary<Animal, AmountChange>>();

    // Weather
    private Weather weather = new Weather();

    private void Start()
    {
        HexCell myCell = transform.GetComponent<HexCell>();
        for (int direction = (int)HexDirection.NE; direction <= (int)HexDirection.NW; ++direction)
        {
            HexCell neighborCell = myCell.GetNeighbor((HexDirection)direction);
            if(neighborCell != null)
                neighbors.Add(neighborCell.transform.GetComponent<Area>());
        }
        foreach (Building building in buildings)
        {
            building.applied();
        }
        markBasement.SetActive(isBasement());
    }
    public int getSpeciesAmount(Animal animal)
    {
        return animalAmounts.ContainsKey(animal) ? animalAmounts[animal].old : 0;
    }
    public int getSpeciesChange(Animal animal)
    {
        return animalAmounts.ContainsKey(animal) ? animalAmounts[animal].change : 0;
    }
    public void changeSpeciesAmount(Animal animal, int change)
    {
        if(animalAmounts.ContainsKey(animal))
        {
            animalAmounts[animal].addCurrent(change);
        } else if(change > 0)
        {
            animalAmounts.Add(animal, new AmountChange(change));
        }
    }
    public void dayIdle()
    {
        weather.judgeWeather();
        switch(weather.GetWeatherType())
        {
            case Weather.WeatherType.Rainy:
                rainFX.SetActive(true);
                snowFX.SetActive(false);
                break;
            case Weather.WeatherType.Snowy:
                rainFX.SetActive(false);
                snowFX.SetActive(true);
                break;
            default:
                rainFX.SetActive(false);
                snowFX.SetActive(false);
                break;
        }
        foreach (Building building in buildings)
        {
            building.idle();
        }
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            animalAndAmount.Value.recordChange();
            animalAndAmount.Key.idle(this, animalAndAmount.Value.old);
        }
        //show specialist mark // will be upgraded to show count in future
        if(getSpecialistsInArea().Count > 0)
        {
            markSpecialist.SetActive(true);
        } else
        {
            markSpecialist.SetActive(false);
        }
    }
    public List<Specialist> getSpecialistsInArea()
    {
        List<Specialist> list = new List<Specialist>();
        foreach(Specialist specialist in Stage.GetSpecialists())
        {
            if(specialist.getCurrentArea().Equals(this))
            {
                list.Add(specialist);
            }
        }
        return list;
    }
    public bool isBasement()
    {
        return Stage.getBaseArea().Equals(this);
    }
    public void addReservation()
    {
        Dictionary<Animal, AmountChange> copy = new Dictionary<Animal, AmountChange>(animalAmounts);
        animalAmountsRecords.AddLast(copy);
    }
    public int getSpeciesAmountInLatestRecord(Animal animal)
    {
        Dictionary<Animal, AmountChange> latestRecord = animalAmountsRecords.Last.Value;
        return latestRecord.ContainsKey(animal) ? latestRecord[animal].old : 0;
    }
    public int getSpeciesChangeInLatestRecord(Animal animal)
    {
        Dictionary<Animal, AmountChange> latestRecord = animalAmountsRecords.Last.Value;
        return latestRecord.ContainsKey(animal) ? latestRecord[animal].change : 0;
    }
    public Weather GetWeather()
    {
        return weather;
    }
    public void AddEnabledAction(Action action)
    {
        enabledActions.Add(action);
    }
    public List<Action> GetEnabledActions()
    {
        return enabledActions;
    }
    public List<Area> GetNeighborAreas()
    {
        return neighbors;
    }
    //foods - animals as food
    //energyNeedsForOneEater - energy one eater requires
    //units - amount of eater
    //returns - satisfied amount
    public int ProvideFood(List<Animal> foods, int energyNeedsForOneEater, int units)
    {
        if(units <= 0) {
            return 0;
        }
        int sumProvidableAmount = 0;
        bool satisfied = false;
        foreach(KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            if (!foods.Contains(animalAndAmount.Key)) //skip non-food animals
                continue;
            int providableAmount = animalAndAmount.Value.old * animalAndAmount.Key.energyAsFood / energyNeedsForOneEater;
            if(sumProvidableAmount + providableAmount >= units)
            {
                providableAmount = units - sumProvidableAmount;
                satisfied = true;
            }
            sumProvidableAmount += providableAmount;
            animalAndAmount.Value.addCurrent(-providableAmount); //decrease eaten animals
            if (satisfied)
                break;
        }
        return sumProvidableAmount;
    }
    public int GetProvidableFoodEnergy(List<Animal> foods, int energyNeedsForOneEater)
    {
        int sumProvidableAmount = 0;
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            if (!foods.Contains(animalAndAmount.Key)) //skip non-food animals
                continue;
            int providableAmount = animalAndAmount.Value.old * animalAndAmount.Key.energyAsFood / energyNeedsForOneEater;
            sumProvidableAmount += providableAmount;
        }
        return sumProvidableAmount;
    }
}
