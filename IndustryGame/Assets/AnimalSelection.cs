using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class AnimalSelection : MonoBehaviour
{
    [HideInInspector]
    public Animal animal;
    [HideInInspector]
    public Button button;

    public Text AnimalName; //TODO: replace with picture;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => Stage.ShowAnimalNumberPop(animal));

        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Deselect;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(DeselectAnimal);

        trigger.triggers.Add(entry);
    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        AnimalName.text = animal.animalName;
    }

    private void DeselectAnimal(BaseEventData pointData)
    {
        Stage.ShowAnimalNumberPop(null);
    }
}
