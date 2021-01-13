using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Base : MonoBehaviour
{
    int money;
    int time;
    LinkedList<Specialist> specialists = new LinkedList<Specialist>();
    LinkedList<Action> enabledActions = new LinkedList<Action>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
