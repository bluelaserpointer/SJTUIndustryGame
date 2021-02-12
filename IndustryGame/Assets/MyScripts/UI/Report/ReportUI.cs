using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportUI : MonoBehaviour
{
    public static ReportUI instance;
    [HideInInspector]
    public static GameObject OpenWindow;

    // 选择窗口
    [Header("生成的事件选择窗口")]
    public GameObject EventWindow;
    [Header("生成的动物选择窗口")]
    public GameObject AnimalWindow;
    [Header("生成的环境选择窗口")]
    public GameObject EnvironmentWindow;


    // 详情页窗口
    [Header("生成的事件详情窗口")]
    public GameObject EventDetailsWindow;
    [Header("生成的动物详情窗口")]
    public GameObject AnimalDetailsWindow;
    [Header("生成的环境详情窗口")]
    public GameObject EnvironmentDetailsWindow;


    [Header("生成的选择窗口位置")]
    public GameObject GeneratePosition;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        GenerateEventWindow();
    }

    void OnDisable()
    {
        ClearWindow();
    }

    public static void GenerateEventWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.EventWindow, instance.GeneratePosition.transform, false);
    }

    public static void GenerateAnimalWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.AnimalWindow, instance.GeneratePosition.transform, false);
    }

    public static void GenerateEnvironmentWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.EnvironmentWindow, instance.GeneratePosition.transform, false);
    }

    public static void GenerateEventDetailsWindow(MainEvent mainEvent)
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.EventDetailsWindow, instance.GeneratePosition.transform, false);
        OpenWindow.GetComponent<EventReportUI>().eventDetails = mainEvent;
    }

    public static void GenerateAnimalDetailsWindow(Animal animal)
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.AnimalDetailsWindow, instance.GeneratePosition.transform, false);
        OpenWindow.GetComponent<AnimalReportUI>().animal = animal;
    }

    public static void GenerateEnvironmentDetailsWindow(Region region)
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.EnvironmentDetailsWindow, instance.GeneratePosition.transform, false);
        OpenWindow.GetComponent<EnvironmentReportUI>().region = region;
    }

    public static void ClearWindow()
    {
        Destroy(OpenWindow);
    }


}
