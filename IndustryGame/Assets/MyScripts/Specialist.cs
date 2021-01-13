using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Specialist : MonoBehaviour
{
    string specialistName;
    int hireCost;
    Dictionary<Ability, int> abilities = new Dictionary<Ability, int>();
    Area currentArea;
    Action currentAction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
