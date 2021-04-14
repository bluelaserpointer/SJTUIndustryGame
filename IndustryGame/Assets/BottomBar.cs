using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour
{
    public static BottomBar instance;
    public Button BuildingsButton;
    public Button SpecialistButton;

    public Color TabChosenColor;
    public Color TabUnchosenColor;

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
        SpecialistButton.onClick.AddListener(() => ActivateSpecialistBar());
        BuildingsButton.onClick.AddListener(() => ActivateBuildingsBar());
    }

    public void ActivateSpecialistBar()
    {
        SpecialistBar.instance.gameObject.SetActive(true);
        BuildingsBar.instance.gameObject.SetActive(false);
        SpecialistButton.image.color = TabChosenColor;
        BuildingsButton.image.color = TabUnchosenColor;

    }

    public void ActivateBuildingsBar()
    {
        SpecialistBar.instance.gameObject.SetActive(false);
        BuildingsBar.instance.gameObject.SetActive(true);
        SpecialistButton.image.color = TabUnchosenColor;
        BuildingsButton.image.color = TabChosenColor;
    }
}
