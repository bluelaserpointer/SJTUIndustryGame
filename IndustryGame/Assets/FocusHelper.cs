using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusHelper : MonoBehaviour
{
    public Color SelectedColor;
    public Color NormalColor;


    private Image SelectedButton;
    public Image StartSelectedButton;

    void Start()
    {
        SelectImage(StartSelectedButton);
    }

    public void SelectImage(Image SelectButton)
    {
        ClearSelection();
        SelectedButton = SelectButton;
        SelectedButton.color = SelectedColor;
    }

    public void ClearSelection()
    {
        if (SelectedButton != null)
        {
            SelectedButton.color = NormalColor;
        }
    }
}
