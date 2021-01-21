using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCanvas : MonoBehaviour
{
    public static PopUpCanvas instance;

    private static bool windowExists = false;

    public GameObject SinglePopUpWindowPrefab;
    private Queue<IPopUpWindow> PopUpWindowQueue;

    void Awake()
    {
        if (instance == null)
            instance = this;
        PopUpWindowQueue = new Queue<IPopUpWindow>();
    }
    public void Start ()
    {
        //GenerateNewPopUpWindow("TEST", "TESTING");
    }

    public static void GenerateNewPopUpWindow(IPopUpWindow window)
    {
        instance.PopUpWindowQueue.Enqueue(window);

        //instance.PopUpWindowQueue.Enqueue(clone);
        if (!windowExists)
        {
            ShowPopUpWindowStack();
        }
        //InGameLog.AddLog(GameObject.FindGameObjectWithTag("PopUpWindow").name);

    }

    public static void ShowPopUpWindowStack ()
    {
        if (instance.PopUpWindowQueue.Count > 0)
        {
            IPopUpWindow topWindow = instance.PopUpWindowQueue.Dequeue();
            topWindow.Generate();
            windowExists = true;
        }
    }

    public static void SetWindowExists (bool cond)
    {
        windowExists = cond;
    }

}