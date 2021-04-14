using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCanvas : MonoBehaviour
{
    public static PopUpCanvas instance;

    private static bool windowExists = false;

    private static readonly Queue<IPopUpWindow> popUpWindowQueue = new Queue<IPopUpWindow>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void Start ()
    {
        //GenerateNewPopUpWindow("TEST", "TESTING");
    }

    public static void GenerateNewPopUpWindow(IPopUpWindow window)
    {
        popUpWindowQueue.Enqueue(window);
        ShowPopUpWindowStack();

        //InGameLog.AddLog(GameObject.FindGameObjectWithTag("PopUpWindow").name);
    }

    public static void ShowPopUpWindowStack ()
    {
        if (!windowExists && popUpWindowQueue.Count > 0)
        {
            IPopUpWindow topWindow = popUpWindowQueue.Dequeue();
            topWindow.Generate();
            windowExists = true;
            Timer.Pause();
        }
    }

    public static void anWindowClosed ()
    {
        windowExists = false;
        Timer.Resume();
        ShowPopUpWindowStack();
    }

}