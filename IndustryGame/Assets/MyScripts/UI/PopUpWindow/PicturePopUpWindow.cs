using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicturePopUpWindow : IPopUpWindow
{
    private string title;
    private string contents;
    private Sprite picture;

    public PicturePopUpWindow (string title, string contents , Sprite picture )
    {
        this.title = title;
        this.contents = contents;
        this.picture = picture;
    }

    public void Generate ()
    {
        GameObject clone = GameObject.Instantiate(PopUpCanvas.instance.PicturePopUpWindowPrefab, PopUpCanvas.instance.transform, false);
        clone.GetComponent<SinglePopUpWindow>().TitleText.text = title;
        clone.GetComponent<SinglePopUpWindow>().ContentsText.text = contents;
        clone.GetComponent<SinglePopUpWindow>().Picture.sprite = picture;
    }
}
