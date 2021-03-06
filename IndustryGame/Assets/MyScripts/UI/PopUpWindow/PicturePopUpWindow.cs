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
        GameObject window = Object.Instantiate(Resources.Load<GameObject>("UI/PopUpWindow/PicturePopUpWindowPrefab"), PopUpCanvas.instance.transform, false);
        SinglePopUpWindow script = window.GetComponent<SinglePopUpWindow>();
        script.TitleText.text = title;
        script.ContentsText.text = contents;
        script.Picture.sprite = picture;
    }
}
