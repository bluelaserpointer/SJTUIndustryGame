using System.Collections.Generic;
using UnityEngine;

public class Specialist
{
    public string name;
    public string birthday;
    public SpecialistTemplate specialistTemplate; //icon, jender, indoor/outdoor
    public int hireCost;
    public Ability speciality;
    public Dictionary<Ability, int> abilities = new Dictionary<Ability, int>();

    Area currentArea;
    Action currentAction;

    public int addSpeciality_range(Ability ability, int minValueInclude, int maxValueInclude)
    {
        System.Random random = new System.Random();
        int level = random.Next(minValueInclude, maxValueInclude + 1);
        if(level <= 0) {
            return 0;
        }
        abilities.Add(ability, level);
        return level;
    }
}
