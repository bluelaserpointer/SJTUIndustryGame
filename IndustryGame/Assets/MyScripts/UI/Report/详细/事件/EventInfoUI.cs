using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventInfoUI : MonoBehaviour
{
    [HideInInspector]
    public EventInfo eventInfo;
    [Header("显示EventInfo标题的Text")]
    public Text InfoName;
    [Header("显示EventInfo描述的Text")]
    public Text Description;

    public void Generate(EventInfo eventInfo)
    {
        this.eventInfo = eventInfo;
        InfoName.text = eventInfo.infoName;
        Description.text = eventInfo.description;
    }

}
