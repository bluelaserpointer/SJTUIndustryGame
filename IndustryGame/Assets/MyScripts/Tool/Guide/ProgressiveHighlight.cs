using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ProgressiveHighlight : MonoBehaviour
{
    public string staticName;
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
    private static List<ProgressiveHighlight> staticInstances = new List<ProgressiveHighlight>();
    public bool IsHighlighted => isHighlighted;
    private GameObject generatedHighlight;
    private List<ProgressiveHighlight> eraseHighlightsAfterClick = new List<ProgressiveHighlight>();

    static ProgressiveHighlight()
    {
        staticInstances.Clear();
    }
    private void Awake()
    {
        if(staticName.Length > 0)
            staticInstances.Add(this);
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
