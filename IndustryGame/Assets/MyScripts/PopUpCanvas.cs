using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCanvas : MonoBehaviour
{
    

    public GameObject SinglePopUpWindowPrefab;

    public void Start ()
    {
        GenerateNewPopUpWindow("TEST", "TESTING");
    }

    public void GenerateNewPopUpWindow(string title,string contents)
    {
        GameObject clone = Instantiate(SinglePopUpWindowPrefab, gameObject.transform, false);
        clone.GetComponent<SinglePopUpWindow>().title= title;
        clone.GetComponent<SinglePopUpWindow>().contents = contents;
    }

}