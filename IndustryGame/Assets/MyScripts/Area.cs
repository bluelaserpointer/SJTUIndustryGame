using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Area : MonoBehaviour
{
    public string areaName;
    [TextArea]
    public string description;
    public EnvironmentType environmentType;
    private Dictionary<Animal, int> animalAmounts = new Dictionary<Animal, int>();

    // Update is called once per frame
    void Update()
    {
        foreach(KeyValuePair<Animal, int> animalAndAmount in animalAmounts)
        {
            animalAndAmount.Key.idle(this, animalAndAmount.Value);
        }
    }

    public void changeSpecieAmount(Animal animal, int change)
    {
        if(animalAmounts.ContainsKey(animal))
        {
            animalAmounts[animal] = Mathf.Max(0, animalAmounts[animal] + change);
        } else if(change > 0)
        {
            animalAmounts.Add(animal, change);
        }
    }
}
