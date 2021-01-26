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
    float actionProgress;

    public void dayIdle()
    {
        if(currentAction != null)
        {
            actionProgress += 1.0f * (1.0f + Stage.GetResourceValue(ResourceType.specialistTrainBoost));
            if(actionProgress >= currentAction.timeCost)
            {
                currentAction.finishAction(currentArea);
                currentAction = null;
                exp += 10; //TODO; need experience info
            }
        }
    }
    /// <summary>
    /// Add an ability by level between minValueInclude ~ maxValueInclude.
    /// Supports removing an ability when its level reach 0.
    /// </summary>
    /// <returns>increased level</returns>
    public int addSpeciality_randomRange_getIncrease(Ability ability, int minValueInclude, int maxValueInclude)
    {
        int oldAbilityLevel, newAbilityLevel;
        int increase = Random.Range(minValueInclude, maxValueInclude + 1);
        if (abilities.ContainsKey(ability)) {
            oldAbilityLevel = abilities[ability];
            int result = oldAbilityLevel + increase;
            if (result <= 0)
            {
                abilities.Remove(ability);
                newAbilityLevel = 0;
            } else {
                if (result > 10)
                {
                    result = 10;
                }
                newAbilityLevel = abilities[ability] = result;
            }
        } else {
            oldAbilityLevel = 0;
            if (increase <= 0)
            {
                newAbilityLevel = 0;
            } else {
                abilities.Add(ability, increase);
                newAbilityLevel = increase;
            }
        }
        return newAbilityLevel - oldAbilityLevel;
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
    public float getActionProgressRate()
    {
        return currentAction != null ? actionProgress / currentAction.timeCost : 1.0f;
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
    /// <summary>
    /// Exp points to get next level. If reaches max level, it returns previous cap;
    /// </summary>
    public int GetExpCap()
    {
        return GetLevel() < expCaps.Length ? expCaps[GetLevel()] : expCaps[expCaps.Length - 1];
    }
    public int GetExp()
    {
        return exp;
    }
    /// <summary>
    /// Percentage of exp
    /// </summary>
    /// <returns>0.0 ~ 1.0</returns>
    public float GetExpRate()
    {
        int level = GetLevel();
        if(level == GetMaxLevel())
            return 1.0f;
        int prevExpCap = expCaps[level - 1];
        return (exp - prevExpCap) / (expCaps[level] - prevExpCap);
    }
    public static int GetMaxLevel()
    {
        return expCaps.Length - 1;
    }
    public int GetAbilityLevel(Ability ability)
    {
        return abilities.ContainsKey(ability) ? abilities[ability] : 0;
    }
    public void AddExp(int value)
    {
        if(exp + value > GetExpCap()) //level up
        {
            string levelUpMessage = "在 " + getCurrentArea().areaName + " 工作的 " + name + " 获得了经验";
            Ability ability = EnumHelper.GetRandomValue<Ability>();
            int oldLevel = GetAbilityLevel(ability);
            int increase = addSpeciality_randomRange_getIncrease(EnumHelper.GetRandomValue<Ability>(), 2, 2);
            if(increase > 0)
            {
                levelUpMessage += "\n" + AbilityDescription.GetAbilityDescription(ability) + ": " + oldLevel + " -> " + GetAbilityLevel(ability);
            }
            PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow("专家升级", levelUpMessage));
        }
        exp += value;
    }
}
