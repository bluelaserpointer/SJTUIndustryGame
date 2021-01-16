using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Area : MonoBehaviour
{
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

    // Weather
    private Weather weather = new Weather();

    private void Start()
    {
        foreach (Building building in buildings)
        {
            building.applied();
        }
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
        foreach (Building building in buildings)
        {
            building.idle();
        }
        foreach (KeyValuePair<Animal, AmountChange> animalAndAmount in animalAmounts)
        {
            animalAndAmount.Value.recordChange();
            animalAndAmount.Key.idle(this, animalAndAmount.Value.old);
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
}
