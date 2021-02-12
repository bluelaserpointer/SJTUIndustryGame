using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    private static WindowsManager instance;

    public GameObject SpecialistWindowPrefab;
    public GameObject ReportWindowPrefab;
    public GameObject SettingsWindowPrefab;

    public static GameObject OpenWindow;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public static bool CheckOpenWindow()
    {
        if (instance == null)
        {
            return false;
        }

        return OpenWindow != null || GameObject.FindGameObjectWithTag("PopUpWindow") != null;
    }

    public void GenerateReportWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.ReportWindowPrefab, instance.gameObject.transform, true);
    }

    public void GenerateSpecialistWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.SpecialistWindowPrefab, instance.gameObject.transform, true);
    }

    public void GenerateSettingsWindow()
    {
        if (OpenWindow != null)
        {
            ClearWindow();
        }
        OpenWindow = Instantiate(instance.SettingsWindowPrefab, instance.gameObject.transform, true);
    }

    public static void ClearWindow()
    {
        Destroy(OpenWindow);
    }
}
