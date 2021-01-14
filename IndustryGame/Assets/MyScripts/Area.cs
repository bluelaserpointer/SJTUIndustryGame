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
}
