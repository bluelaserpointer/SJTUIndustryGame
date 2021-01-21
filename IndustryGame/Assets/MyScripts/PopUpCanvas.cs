using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCanvas : MonoBehaviour
{
    private static PopUpCanvas instance;

    public GameObject SinglePopUpWindowPrefab;
    private Stack<GameObject> PopUpWindowStack;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void Start ()
    {
        //GenerateNewPopUpWindow("TEST", "TESTING");
    }

    public static void GenerateNewPopUpWindow(string title,string contents)
    {
        GameObject clone = Instantiate(instance.SinglePopUpWindowPrefab, instance.transform, false);
        clone.GetComponent<SinglePopUpWindow>().title= title;
        clone.GetComponent<SinglePopUpWindow>().contents = contents;
        clone.SetActive(false);
        instance.PopUpWindowStack.Push(clone);
    }

    public static void ShowPopUpWindowStack ()
    {
        while (instance.PopUpWindowStack.Count > 0)
        {
            GameObject topWindow = instance.PopUpWindowStack.Pop();
            topWindow.SetActive(true);
        }
    }

}