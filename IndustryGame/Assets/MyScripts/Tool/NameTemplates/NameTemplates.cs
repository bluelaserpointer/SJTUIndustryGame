using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/NameTemplates")]
public class NameTemplates : ScriptableObject
{
    [SerializeField] private List<string> names;
    private readonly List<string> unusedNames = new List<string>();
    private NameTemplates() {}
    /// <summary>
    /// Cause used names could reappear
    /// </summary>
    public void Reset()
    {
        unusedNames.Clear();
        unusedNames.AddRange(names);
    }
    /// <summary>
    /// Cause all <see cref="NameTemplates"/> used names could reappear
    /// </summary>
    public static void ResetAll()
    {
        foreach(NameTemplates nameTemplates in Resources.LoadAll<NameTemplates>("NameTemplates"))
        {
            nameTemplates.Reset();
        }
    }
    public string PickRandomOne()
    {
        if (names.Count == 0)
            return "defaultName";
        if(unusedNames.Count > 0) //如果语料库充足则尽量返回不同单词，超过则随机返回
        {
            int randomIndex = Random.Range(0, unusedNames.Count);
            string name = unusedNames[randomIndex];
            unusedNames.RemoveAt(randomIndex);
            return name;
        }
        return names[Random.Range(0, names.Count)];
    }
}
