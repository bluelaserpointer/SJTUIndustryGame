using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Stage : MonoBehaviour
{
    private LinkedList<Area> areas = new LinkedList<Area>();

    public string stageName;
    [TextArea]
    public string description;
    [Header("Money allowed for this stage")]
    public int stageMoney;
    [Header("Time allowed for this stage")]
    public int stageTime;
    public List<Event> events;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: collect Area links
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
