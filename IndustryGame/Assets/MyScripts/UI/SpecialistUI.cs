using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialistUI : MonoBehaviour
{
    public GameObject SpecialistList;
    public GameObject SingleSpecialistPrefab;
    private List<Specialist> specialists;

    void Start ()
    {
        specialists = Stage.GetSpecialists();

        InstantiateSpecialistList();
    }

    void Update()
    {
        specialists = Stage.GetSpecialists();
    }

    private void OnEnable ()
    {
        InstantiateSpecialistList();
        InGameLog.AddLog("In SpecialistUI OnEnable");

    }

    void InstantiateSpecialistList ()
    {
        for (int i = 0 ; i < specialists.Count ; i++)
        {
            GameObject clone;
            clone = Instantiate(SingleSpecialistPrefab, SpecialistList.transform, false);
            clone.GetComponent<SingleSpecialist>().specialist = specialists[i];
            InGameLog.AddLog("Ins : " + i);
        }
    }

   
}
