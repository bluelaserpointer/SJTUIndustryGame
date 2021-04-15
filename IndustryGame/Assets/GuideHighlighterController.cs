using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GuideHighlighterController : MonoBehaviour
{
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetGuideHighliter(RectTransform rect)
    {
        rectTransform.SetParent(rect.parent);
        rectTransform.anchoredPosition = rect.anchoredPosition;
        rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x + 1.0f, rect.sizeDelta.y + 1.0f);
    }
}
