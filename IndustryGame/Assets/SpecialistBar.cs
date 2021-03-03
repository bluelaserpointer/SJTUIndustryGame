using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpecialistBar : MonoBehaviour
{
    public static SpecialistBar instance;

    [Header("生成专家图片位置")]
    public GameObject GenerateSpecialistImagePosition;
    [Header("生成的专家Prefab")]
    public GameObject SpecialistImagePrefab;

    private List<GameObject> GeneratedSpecialists = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        RefreshList();
    }

    public void RefreshList()
    {
        Helper.ClearList(GeneratedSpecialists);
        foreach (Specialist specialist in Stage.GetSpecialists())
        {
            GameObject clone = Instantiate(SpecialistImagePrefab, GenerateSpecialistImagePosition.transform, false);
            clone.GetComponent<SingleBarSpecialist>().RefreshUI(specialist);
            GeneratedSpecialists.Add(clone);
        }
    }


}
