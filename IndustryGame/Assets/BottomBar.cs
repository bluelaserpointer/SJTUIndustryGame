using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBar : MonoBehaviour
{
    public static BottomBar instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        ActivateSpecialistBar();
    }

    public void ActivateSpecialistBar()
    {
        SpecialistBar.instance.gameObject.SetActive(true);
        BuildingsBar.instance.gameObject.SetActive(false);
    }

    public void ActivateBuildingsBar()
    {
        SpecialistBar.instance.gameObject.SetActive(false);
        BuildingsBar.instance.gameObject.SetActive(true);
    }
}
