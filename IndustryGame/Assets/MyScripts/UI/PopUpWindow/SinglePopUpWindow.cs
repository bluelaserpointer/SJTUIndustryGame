using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SinglePopUpWindow : MonoBehaviour
{
    public Text TitleText;
    public Text ContentsText;
    public Image Picture;

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
