using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewsImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject NewsDescriptionPanel;
    public GameObject CloseButton;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        NewsDescriptionPanel.SetActive(true);
        CloseButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NewsDescriptionPanel.SetActive(false);
        CloseButton.SetActive(false);
    }

}
