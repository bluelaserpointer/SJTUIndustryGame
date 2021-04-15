using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ProgressiveHighlight : MonoBehaviour
{
    public GameObject highlightPrefab;
    [SerializeField]
    private List<Button> buttons;
    [SerializeField]
    private bool highlightOnAwake;
    private bool isHighlighted;
    public List<ProgressiveHighlight> nextHighlights;
    public UnityEvent highlightEvent;
    public UnityEvent stopHighlightEvent;

    //data
    public bool IsHighlighted => isHighlighted;
    private GameObject generatedHighlight;
    private List<ProgressiveHighlight> eraseHighlightsAfterClick = new List<ProgressiveHighlight>();

    private void Awake()
    {
        if (highlightOnAwake)
            Highlight();
        buttons.ForEach(eachButton => eachButton.onClick.AddListener(Click));
    }
    public void Click()
    {
        if (isHighlighted)
        {
            //stop old highlights includes me
            eraseHighlightsAfterClick.ForEach(eachHighlight => eachHighlight.StopHighlight());
            if(isHighlighted)
            {
                StopHighlight();
            }
            //start next highlights
            foreach (ProgressiveHighlight eachOne in nextHighlights)
            {
                eachOne.Highlight();
                eachOne.eraseHighlightsAfterClick.AddRange(nextHighlights);
            }
        }
    }
    public void Highlight()
    {
        if(!isHighlighted)
        {
            isHighlighted = true;
            generatedHighlight = Instantiate(highlightPrefab, transform);
            highlightEvent.Invoke();
        }
    }
    public void StopHighlight()
    {
        if(isHighlighted)
        {
            isHighlighted = false;
            stopHighlightEvent.Invoke();
            Destroy(generatedHighlight);
        }
    }
}
