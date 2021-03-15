using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelFocusHelper : MonoBehaviour
{
    public Color ImageSelectedColor;
    public Color ImageNormalColor;
    public Color TextNormalColor;
    public Color TextSelectedColor;

    private Image SelectedButton;
    private Text SelectedText;


    public void SelectImage(Image SelectButton, Text SelectText)
    {
        ClearSelection();
        SelectedButton = SelectButton;
        SelectedText = SelectText;
        SelectedButton.color = ImageSelectedColor;
        SelectedText.color = TextSelectedColor;
    }

    public void ClearSelection()
    {
        if (SelectedButton != null)
        {
            SelectedButton.color = ImageNormalColor;
            SelectedText.color = TextNormalColor;
        }
    }
}
