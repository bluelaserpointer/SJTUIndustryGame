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
        GameObject window = Object.Instantiate(Object.Instantiate(Resources.Load<GameObject>("UI/PopUpWindow/SimplePopUpWindowPrefab")), PopUpCanvas.instance.transform, false);
        SinglePopUpWindow script =  window.GetComponent<SinglePopUpWindow>();
        script.TitleText.text = title;
        script.ContentsText.text = contents;
    }
}
