using System.Collections.Generic;
using UnityEngine;


public class SpecialistUI : MonoBehaviour
{
    public GameObject SpecialistList;
    public GameObject SingleSpecialistPrefab;
    private List<GameObject> Specialists = new List<GameObject>();

    void Start()
    {
        InstantiateSpecialistList();
    }


    private void OnEnable()
    {
        InstantiateSpecialistList();
        // InGameLog.AddLog("In SpecialistUI OnEnable");

    }

    void InstantiateSpecialistList()
    {
        Helper.ClearList(Specialists);
        foreach (Specialist specialist in Stage.GetSpecialists())
        {
            GameObject clone = Instantiate(SingleSpecialistPrefab, SpecialistList.transform, false);
            clone.GetComponent<SingleSpecialist>().specialist = specialist;
            Specialists.Add(clone);
        }
    }


}
