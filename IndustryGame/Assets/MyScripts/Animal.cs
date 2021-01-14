using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Animal")]
public class Animal : ScriptableObject
{
    public string animalName;
    [TextArea]
    public string description;
    public GameObject model;
    [Serializable]
    public struct SpeciesAffect
    {
        public Animal target;
        public int change;
    }
    public List<SpeciesAffect> speciesAffects;
    public void idle(Area area, int amount)
    {
    }
}
