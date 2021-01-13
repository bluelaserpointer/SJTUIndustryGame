using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Area : MonoBehaviour
{
    public string areaName;
    [TextArea]
    public string description;
    private Dictionary<Animal, int> animalAmounts = new Dictionary<Animal, int>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
