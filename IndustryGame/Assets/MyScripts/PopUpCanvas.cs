using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCanvas : MonoBehaviour
{
    private static PopUpCanvas instance;

    public GameObject SinglePopUpWindowPrefab;

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
    }

}