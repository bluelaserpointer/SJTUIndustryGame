using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    /// <summary>
    /// 是否被鼠标悬浮
    /// </summary>
    private bool isPointerEntered;
    public bool IsPointerEntered { get { return isPointerEntered; } }
    /// <summary>
    /// 受控的ToolTip
    /// </summary>
    public GameObject Tooltip { get { return tooltip; } }
    private void Awake()
    {
        tooltip.SetActive(false);
    }
    private void OnDisable()
    {
        tooltip.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(isPointerEntered = true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(isPointerEntered = false);
    }
}
