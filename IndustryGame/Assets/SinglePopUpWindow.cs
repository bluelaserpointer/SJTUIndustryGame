using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SinglePopUpWindow : MonoBehaviour
{
    public string title;
    public string contents;

    public Text TitleText;
    public Text ContentsText;

    void Update()
    {
        TitleText.text = title;
        ContentsText.text = contents;
    }

    private void OnDisable ()
    {
        CloseWindow();
    }

    public void CloseWindow ()
    {
        Destroy(gameObject);
    }
}
