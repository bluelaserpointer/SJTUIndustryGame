﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpecialistUI : MonoBehaviour
{
    public GameObject SpecialistList;
    public GameObject SingleSpecialistPrefab;
    private List<Specialist> specialists;

    // Start is called before the first frame update
    void Start ()
    {
       
    }

    void Update()
    {
        specialists = Stage.GetSpecialists();
    }

    private void OnEnable ()
    {
        InstantiateSpecialistList();
    }

    void InstantiateSpecialistList ()
    {
        for (int i = 0 ; i < specialists.Count ; i++)
        {
            GameObject clone;
            clone = Instantiate(SingleSpecialistPrefab, SpecialistList.transform, false);
            clone.GetComponent<SingleSpecialist>().specialist = specialists[i];
        }
    }

   
}
