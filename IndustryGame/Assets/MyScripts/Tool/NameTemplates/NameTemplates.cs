using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/NameTemplates")]
public class NameTemplates : ScriptableObject
{
    [SerializeField] private List<string> names;
    private bool backupTaken = false;
    private List<string> originalNames;
    private NameTemplates() {}
    public string PickRandomOne()
    {
        if(!backupTaken)
        {
            backupTaken = true;
            originalNames = new List<string>(names);
        }
        if (originalNames.Count == 0)
            return "defaultName";
        if(names.Count > 0) //如果语料库充足则尽量返回不同单词，超过则随机返回
        {
            int randomIndex = Random.Range(0, names.Count);
            string name = names[randomIndex];
            names.RemoveAt(randomIndex);
            return name;
        }
        return originalNames[Random.Range(0, originalNames.Count)];
    }
}
