using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class GuideTextDisplay : MonoBehaviour
{
    public static GuideTextDisplay instance;
    private Text textUI;
    private List<string> texts = new List<string>();
    private void Awake()
    {
        textUI = GetComponent<Text>();
        textUI.text = "";
        if (instance == null)
            instance = this;
    }
    public void AddText(string text)
    {
        if (text.Length == 0)
            return;
        texts.Add(text);
        UpdateTextUI();
    }
    public void RemoveText(string text)
    {
        if (text.Length == 0)
            return;
        foreach (string eachText in texts)
        {
            if(eachText.Equals(text))
            {
                texts.Remove(text);
                UpdateTextUI();
                break;
            }
        }
    }
    private void UpdateTextUI()
    {
        string str = "";
        texts.ForEach(eachText => str += "\n- " + eachText);
        textUI.text = str;
    }
}
