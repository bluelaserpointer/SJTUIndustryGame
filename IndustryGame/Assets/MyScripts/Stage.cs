using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Stage : MonoBehaviour
{
    private static Stage instance;

    private LinkedList<Area> areas = new LinkedList<Area>();

    public string stageName;
    [TextArea]
    public string description;
    [Header("Money allowed for this stage")]
    public int stageMoney;
    [Header("Time allowed for this stage")]
    public int stageTime;
    public List<Event> events;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static LinkedList<Area> getAreas()
    {
        return instance.areas;
    }
}
