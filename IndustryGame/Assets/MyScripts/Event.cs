using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public string eventName;
    [TextArea]
    public string description;
    public List<EventInfo> infos = new List<EventInfo>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
