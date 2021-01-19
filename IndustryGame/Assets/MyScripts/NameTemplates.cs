using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/NameTemplates")]
public class NameTemplates : ScriptableObject
{
    [SerializeField] private List<string> names;
    public string pickRandomOne()
    {
        return names.Count > 0 ? names[Random.Range(0, names.Count)] : "defaultName";
    }
}
