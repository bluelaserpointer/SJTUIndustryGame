using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GuideTextDisplay : MonoBehaviour
{
    public static GuideTextDisplay instance;

    public GameObject SingleGuideLinePrefab;

    private List<GameObject> GeneratedGuideLines = new List<GameObject>();

    public GameObject InProgressPanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // eventName - 该操作出自哪一个事件流
    // actionDescription - 应该做的操作提示描述
    public void AddGuideLine(string eventName, string actionDescription)
    {
        Debug.Log("Adding guideline: " + eventName + " - " + actionDescription);
        GameObject clone = Instantiate(SingleGuideLinePrefab, transform, false);
        clone.GetComponent<SingleGuideLinePrefab>().eventName.text = eventName;
        clone.GetComponent<SingleGuideLinePrefab>().actionDescription.text = actionDescription;
        GeneratedGuideLines.Add(clone);
        InProgressPanel.SetActive(true);
    }

    public void RemoveGuideLine(string eventName, string actionDescription)
    {
        Debug.Log("Removing guideline: " + eventName + " - " + actionDescription);

        foreach (GameObject obj in GeneratedGuideLines)
        {
            if (obj.GetComponent<SingleGuideLinePrefab>().actionDescription.text.Equals(actionDescription))
            {
                GeneratedGuideLines.Remove(obj);
                Destroy(obj);
                break;
            }
        }

        if (GeneratedGuideLines.Count == 0)
        {
            InProgressPanel.SetActive(false);
        }
    }

    // public void AddText(string text)
    // {
    //     if (text.Length == 0)
    //         return;
    //     texts.Add(text);
    //     UpdateTextUI();
    // }
    // public void RemoveText(string text)
    // {
    //     if (text.Length == 0)
    //         return;
    //     foreach (string eachText in texts)
    //     {
    //         if (eachText.Equals(text))
    //         {
    //             texts.Remove(text);
    //             UpdateTextUI();
    //             break;
    //         }
    //     }
    // }
    // private void UpdateTextUI()
    // {
    //     string str = "";
    //     texts.ForEach(eachText => str += "\n- " + eachText);
    //     textUI.text = str;


    // }
}
