using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePopUpWindow : IPopUpWindow
{
    private string title;
    private string contents;
    private Sprite picture;

    public SimplePopUpWindow (string title, string contents)
    {
        this.title = title;
        this.contents = contents;
    }

    public void Generate ()
    {
        GameObject clone = GameObject.Instantiate(PopUpCanvas.instance.SimplePopUpWindowPrefab, PopUpCanvas.instance.transform, false);
        clone.GetComponent<SinglePopUpWindow>().TitleText.text = title;
        clone.GetComponent<SinglePopUpWindow>().ContentsText.text = contents;
    }
}
