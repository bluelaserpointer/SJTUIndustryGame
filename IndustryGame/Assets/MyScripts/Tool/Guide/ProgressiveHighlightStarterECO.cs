using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveHighlightStarterECO : MonoBehaviour
{
    public EventStageSO eventStageSO;
    [Serializable]
    public struct HighlightAndText
    {
        public ProgressiveHighlight highlightTarget;
        public string guideText;
    }
    public List<HighlightAndText> highlightAndTexts;
    public void Start()
    {
        eventStageSO.appearEvent.AddListener(Invoke);
        eventStageSO.finishEvent.AddListener(End);
    }
    public void Invoke()
    {
        if (highlightAndTexts.Count == 0)
            return;
        for(int i = 0; i < highlightAndTexts.Count; ++i)
        {
            if (i != highlightAndTexts.Count - 1)
            {
                ProgressiveHighlight highlightTarget = highlightAndTexts[i].highlightTarget;
                string guideText = highlightAndTexts[i].guideText;
                highlightTarget.nextHighlights.Clear();
                if(highlightAndTexts[i + 1].highlightTarget != null)
                    highlightTarget.nextHighlights.Add(highlightAndTexts[i + 1].highlightTarget);
                highlightTarget.highlightEvent.RemoveAllListeners();
                highlightTarget.highlightEvent.AddListener(() => GuideTextDisplay.instance.AddGuideLine("下一步操作", guideText));
                highlightTarget.stopHighlightEvent.RemoveAllListeners();
                highlightTarget.stopHighlightEvent.AddListener(() => GuideTextDisplay.instance.RemoveGuideLine("下一步操作", guideText));
            }
        }
        //highlight first one
        if (highlightAndTexts[0].highlightTarget != null)
            highlightAndTexts[0].highlightTarget.Highlight();
    }
    public void End()
    {
        foreach(HighlightAndText pair in highlightAndTexts)
        {
            if(pair.highlightTarget != null)
                pair.highlightTarget.StopHighlight();
        }
    }
}
