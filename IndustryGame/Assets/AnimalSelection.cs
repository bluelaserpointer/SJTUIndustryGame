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
    public Color NormalColor;
    public Color SelectedColor;
    private Image BackgroundImage;

    void Start()
    {
        BackgroundImage = gameObject.GetComponent<Image>();

        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OpenFilter);

        // EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        // EventTrigger.Entry entry = new EventTrigger.Entry();
        // entry.eventID = EventTriggerType.Deselect;
        // entry.callback = new EventTrigger.TriggerEvent();
        // entry.callback.AddListener(DeselectAnimal);

        // trigger.triggers.Add(entry);
    }

    void Update()
    {
        RefreshUI();
    }

    public void OpenFilter()
    {
        if (Stage.ShowingNumberPopsAnimal == null)
        {
            Stage.ShowAnimalNumberPop(animal);
            BackgroundImage.color = SelectedColor;
        }
        else
        {
            Stage.ShowAnimalNumberPop(null);
            BackgroundImage.color = NormalColor;
        }
    }

    private void RefreshUI()
    {
        AnimalName.text = animal.animalName;
    }


}
