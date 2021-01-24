using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SinglePopUpWindow : MonoBehaviour
{
    public string title;
    public string contents;
    public Sprite picture;

    public Text TitleText;
    public Text ContentsText;
    public Image Picture;

    void Update()
    {
        TitleText.text = title;
        ContentsText.text = contents;
        Picture.sprite = picture;
    }

    private void OnDisable ()
    {
        CloseWindow();
    }

    public void CloseWindow ()
    {
        Destroy(gameObject);
        PopUpCanvas.SetWindowExists(false);
        PopUpCanvas.ShowPopUpWindowStack();
    }
}
