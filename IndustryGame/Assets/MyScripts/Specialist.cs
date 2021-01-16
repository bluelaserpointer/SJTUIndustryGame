using System.Collections.Generic;
using UnityEngine;

public class Specialist
{
    public string name;
    public string birthday;
    public string birthplace;
    public SpecialistTemplate specialistTemplate; //icon, jender, indoor/outdoor
    public int hireCost;
    public Ability speciality;
    public Dictionary<Ability, int> abilities = new Dictionary<Ability, int>();

    Area currentArea;
    Action currentAction;
    double actionProgress;

    public void dayIdle()
    {
        if(currentAction != null)
        {
            actionProgress += 1;
            if(actionProgress >= currentAction.timeCost)
            {
                currentAction.finishAction();
                currentAction = null;
            }
        }
    }
    /*
     *  Add an ability for level between minValueInclude ~ maxValueInclude.
     *  Returns actural level.
     */
    public int addSpeciality_range(Ability ability, int minValueInclude, int maxValueInclude)
    {
        int level = Random.Range(minValueInclude, maxValueInclude + 1);
        if(level <= 0) {
            return 0;
        }
        abilities.Add(ability, level);
        return level;
    }
    public void startAction(Action action)
    {
        currentAction = action;
        actionProgress = 0;
    }
    public void moveToArea(Area area)
    {
        currentArea = area;
    }
}
