using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewsImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject NewsDescriptionPanel;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        NewsDescriptionPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NewsDescriptionPanel.SetActive(false);
    }

}
