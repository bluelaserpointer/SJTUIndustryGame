using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InGameLog : MonoBehaviour
{
    private bool hide = true;
    private string searchText = "";
    private Vector2 scrollViewVector = Vector2.zero;
    private static LinkedList<Log> logs = new LinkedList<Log>();

    struct Log
    {
        public string text;
        public GUIStyle guiStyle;
        public Log(string text)
        {
            this.text = text;
            guiStyle = GUIStyle.none;
        }
        public Log(string text, Color color)
        {
            this.text = text;
            guiStyle = new GUIStyle();
            guiStyle.normal.textColor = color;
        }
    }

    void OnGUI()
    {
        const int baseW = 250, baseH = 20, textH = 15;
        hide = GUI.Toggle(new Rect(0, baseH * 0, 40, baseH), hide, "hide");
        if (!hide)
        {
            GUI.BeginGroup(new Rect(0, 0, baseW, baseH * 9));
            GUI.Box(new Rect(0, 0, baseW, baseH * 9), "");
            GUI.Label(new Rect(0, baseH * 1, 70, baseH), "SearchLog");
            searchText = GUI.TextField(new Rect(70, baseH * 1, baseW - 70, baseH), searchText);
            scrollViewVector = GUI.BeginScrollView(new Rect(0, baseH * 2, baseW, baseH * 6), scrollViewVector, new Rect(0, 0, 400, Mathf.Max(baseH * 3, logs.Count * textH)));
            GUI.BeginGroup(new Rect(0, 0, 400, Mathf.Max(baseH * 3, logs.Count * textH)));
            int y = 0;
            foreach(Log log in logs)
            {
                if(searchText.Length == 0 || log.text.Contains(searchText))
                    GUI.Label(new Rect(0, textH * y++, baseW, textH), log.text, log.guiStyle);
            }
            GUI.EndGroup();
            GUI.EndScrollView();
            if (GUI.Button(new Rect(0, baseH * 8, baseW, baseH), "Clear"))
            {
                logs.Clear();
            }
            GUI.EndGroup();
        }
    }

    public static void AddLog(string log)
    {
        AddLog(log, Color.white);
    }
    public static void AddLog(string log, Color textColor)
    {
        logs.AddFirst(new Log(log, textColor));
    }
}
