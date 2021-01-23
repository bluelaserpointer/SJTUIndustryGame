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
    private int exp;
    public static int[] expCaps = { 0, 100, 200, 500, 1000, 1600, 2500 };

    Area currentArea;
    Action currentAction;
    double actionProgress;

    public void dayIdle()
    {
        if(currentAction != null)
        {
            actionProgress += 1 * (1.0 + Stage.GetResourceValue(ResourceType.specialistTrainBoost));
            if(actionProgress >= currentAction.timeCost)
            {
                currentAction.finishAction(currentArea);
                currentAction = null;
                exp += 10; //TODO; need experience info
            }
        }
    }
    /*
     *  Add an ability for level between minValueInclude ~ maxValueInclude.
     *  Supports removing an ability when its level reach 0.
     *  Returns result level.
     */
    public int addSpeciality_range(Ability ability, int minValueInclude, int maxValueInclude)
    {
        int level = Random.Range(minValueInclude, maxValueInclude + 1);
        if (abilities.ContainsKey(ability))
        {
            int result = Mathf.Clamp(abilities[ability] + level, 0, 10);
            if (result == 0)
            {
                abilities.Remove(ability);
                return 0;
            }
            return abilities[ability] = result;
        }
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
        Stage.subMoney(action.moneyCost);
        currentArea.StartProgressSlider(this);
    }
    public void moveToArea(Area area)
    {
        currentArea = area;
    }
    public Area getCurrentArea()
    {
        return currentArea;
    }
    public double getActionProgressRate()
    {
        return currentAction != null ? actionProgress / currentAction.timeCost : 0.0f;
    }
    public bool hasCurrentAction()
    {
        return currentAction != null;
    }
    public int GetLevel()
    {
        int level = 0;
        foreach (int expCap in expCaps)
        {
            if (exp < expCap)
            {
                break;
            }
            ++level;
        }
        return level;
    }
    public int GetExpCap()
    {
        return expCaps[GetLevel()];
    }
    public double GetExpRate()
    {
        int level = GetLevel();
        if(level == GetMaxLevel())
            return 1.0;
        int prevExpCap = expCaps[level - 1];
        return (exp - prevExpCap) / (expCaps[level] - prevExpCap);
    }
    public static int GetMaxLevel()
    {
        return expCaps.Length - 1;
    }
    public void AddExp(int value)
    {
        if(exp + value > GetExpCap()) //level up
        {
            addSpeciality_range(EnumHelper.GetRandomValue<Ability>(), 2, 2);
            addSpeciality_range(EnumHelper.GetRandomValue<Ability>(), 1, 1);
        }
        exp += value;
    }
}
