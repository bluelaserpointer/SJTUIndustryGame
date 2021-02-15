using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        // button.onClick.AddListener(() => FilterPanel.instance.)
    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        AnimalName.text = animal.animalName;
    }
}
