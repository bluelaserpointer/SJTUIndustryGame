using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventInfoUI : MonoBehaviour
{
    public EventStage eventInfo;
    public Text InfoName;
    public Text Description;

    void Start()
    {
        
    }

    void Update()
    {
        InfoName.text = eventInfo.name;
        Description.text = eventInfo.description;
    }

    private void OnDisable ()
    {
        Destroy(gameObject);
    }
}
