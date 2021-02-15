using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterPanelAnimalPanel : MonoBehaviour
{
    public MainEvent mainEvent;
    public GameObject AnimalSelectionPrefab;

    public GameObject AnimalSelectionGenerationPosition;

    private List<GameObject> GeneratedAnimalSelectionPrefabs = new List<GameObject>();

    public void RefreshUI()
    {
        Helper.ClearList(GeneratedAnimalSelectionPrefabs);
        foreach (Animal animal in mainEvent.concernedAnimals)
        {
            GameObject clone = Instantiate(AnimalSelectionPrefab, AnimalSelectionGenerationPosition.transform, false);
            clone.GetComponent<AnimalSelection>().animal = animal;
            GeneratedAnimalSelectionPrefabs.Add(clone);
        }
    }
}
