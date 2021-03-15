using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
[RequireComponent(typeof(UnityEngine.UI.Button))]
public class TextButtonHelper : MonoBehaviour
{
    public Color NormalColor;
    public Color SelectedColor;

    private Button button;

    public List<Text> ControlledText;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(ChangeToSelectedColor);

        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Deselect;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(ChangeToNormalColor);

        trigger.triggers.Add(entry);
    }

    private void ChangeToSelectedColor()
    {
        foreach (Text text in ControlledText)
        {
            text.color = SelectedColor;
        }
    }

    private void ChangeToNormalColor(BaseEventData pointData)
    {
        foreach (Text text in ControlledText)
        {
            text.color = NormalColor;
        }
    }
}
