using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Animal")]
public class Animal : ScriptableObject
{
    public string animalName;
    [TextArea]
    public string description;
    public EnvironmentType environment;
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
        if (amount == 0)
            return;
        int magnitude = amount;
        //check food requirements satisfied for how many units
        foreach (SpeciesAffect affect in speciesAffects)
        {
            if(affect.change < 0) //minus change means food requirements
            {
                int tmpMagnitude = area.getSpeciesAmount(affect.target) % (-affect.change);
                if(tmpMagnitude < magnitude) //take least magnitude
                {
                    magnitude = tmpMagnitude;
                    if (magnitude == 0)
                        break;
                }
            }
        }
        //do changes
        if (magnitude > 0)
        {
            foreach (SpeciesAffect affect in speciesAffects)
            {
                area.changeSpeciesAmount(affect.target, affect.change * magnitude);
            }
        }
    }
}
