using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Stage : MonoBehaviour
{
    private LinkedList<Event> events = new LinkedList<Event>();
    private LinkedList<Area> areas = new LinkedList<Area>();

    public string stageName;
    [TextArea]
    public string description;
    [Header("Money allowed for this stage")]
    public int stageMoney;

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
