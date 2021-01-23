using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HireUI : MonoBehaviour
{
    public List<Specialist> specialists;
    public GameObject HireProfilePrefab;
    public GameObject HireCanvas;


    
    void Start()
    {
        GetHireList();
    }

    void Update()
    {
        
    }

    public void GetHireList ()
    {
        specialists = SpecialistEmployList.getSpecialists();
        for (int i = 0 ; i < specialists.Count ; i++)
        {
            GameObject clone = Instantiate(HireProfilePrefab, HireCanvas.transform, false);
            HireProfilePrefab.GetComponent<ProfileManage>().specialist = specialists[i];
        }
    }


}
