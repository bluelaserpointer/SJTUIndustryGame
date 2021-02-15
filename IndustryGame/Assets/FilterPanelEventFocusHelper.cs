using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelEventFocusHelper : MonoBehaviour
{
    public Sprite SpriteSelected;
    public Sprite SpriteNormal;

    private Image SelectedButton;
    public void SelectImage(Image SelectButton)
    {
        ClearSelection();
        SelectedButton = SelectButton;
        SelectedButton.sprite = SpriteSelected;
    }

    public void ClearSelection()
    {
        if (SelectedButton != null)
        {
            SelectedButton.sprite = SpriteNormal;
        }
    }
}
