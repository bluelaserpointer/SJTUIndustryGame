using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventInfoUI : MonoBehaviour
{
    public EventInfo eventInfo;
    public Text InfoName;
    public Text Description;

    void Start()
    {
        
    }

    void Update()
    {
        InfoName.text = eventInfo.infoName;
        Description.text = eventInfo.description;
    }

    private void OnDisable ()
    {
        Destroy(gameObject);
    }
}
