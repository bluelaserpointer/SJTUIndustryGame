using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventInfoUI : MonoBehaviour
{
    [HideInInspector]
    public EventStage eventInfo;
    [Header("显示EventInfo标题的Text")]
    public Text InfoName;
    [Header("显示EventInfo描述的Text")]
    public Text Description;
    [Header("显示EventInfo图片的Image")]
    public Image InfoImage;

    public void Generate(EventStage eventInfo)
    {
        this.eventInfo = eventInfo;
        InfoName.text = eventInfo.name;
        Description.text = eventInfo.description;
        InfoImage.sprite = eventInfo.image;
    }

}
