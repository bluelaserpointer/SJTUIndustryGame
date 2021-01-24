using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePopUpWindow : IPopUpWindow
{
    public string title;
    public string contents;

    public SimplePopUpWindow (string title, string contents)
    {
        this.title = title;
        this.contents = contents;
    }

    public void Generate ()
    {
        GameObject clone = GameObject.Instantiate(PopUpCanvas.instance.SinglePopUpWindowPrefab, PopUpCanvas.instance.transform, false);
        clone.GetComponent<SinglePopUpWindow>().title = title;
        clone.GetComponent<SinglePopUpWindow>().contents = contents;
    }
}
