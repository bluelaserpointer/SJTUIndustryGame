using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 专家
/// </summary>
public class Specialist
{
    /// <summary>
    /// 专家名称
    /// </summary>
    public string name;
    /// <summary>
    /// 专家生日
    /// </summary>
    public string birthday;
    /// <summary>
    /// 专家出生地
    /// </summary>
    public string birthplace;
    /// <summary>
    /// 专家模板
    /// </summary>
    public SpecialistTemplate specialistTemplate; //icon, jender, indoor/outdoor
    /// <summary>
    /// 雇佣金额
    /// </summary>
    public int hireCost;
    /// <summary>
    /// 最高专长
    /// </summary>
    public Ability speciality;
    /// <summary>
    /// 专长列表
    /// </summary>
    public Dictionary<Ability, int> abilities = new Dictionary<Ability, int>();
    /// <summary>
    /// 累计经验值
    /// </summary>
    private int exp;
    /// <summary>
    /// 各级别升级所需累计经验值
    /// </summary>
    public static int[] expCaps = { 0, 100, 200, 500, 1000, 1600, 2500 };

    private Area currentArea;
    /// <summary>
    /// 当前所在地区
    /// </summary>
    public Area Area { get { return currentArea; } }
    private SpecialistAction currentAction;
    /// <summary>
    /// 执行中措施
    /// </summary>
    public SpecialistAction Action { get { return currentAction; } }
    /// <summary>
    /// 是否存在执行中措施
    /// </summary>
    public bool HasAction { get { return currentAction != null; } }

    public void dayIdle()
    {
        if (HasAction)
        {
            Stage.AddResourceValue(ResourceType.money, -currentAction.DayMoneyCost);
            currentAction.DayIdle();
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
        if (abilities.ContainsKey(ability))
        {
            oldAbilityLevel = abilities[ability];
            int result = oldAbilityLevel + increase;
            if (result <= 0)
            {
                abilities.Remove(ability);
                newAbilityLevel = 0;
            }
            else
            {
                if (result > 10)
                {
                    result = 10;
                }
                newAbilityLevel = abilities[ability] = result;
            }
        }
        else
        {
            oldAbilityLevel = 0;
            if (increase <= 0)
            {
                newAbilityLevel = 0;
            }
            else
            {
                abilities.Add(ability, increase);
                newAbilityLevel = increase;
            }
        }
        return newAbilityLevel - oldAbilityLevel;
    }
    public void SetAction(SpecialistAction action)
    {
        if (HasAction)
        {
            currentAction.Stop();
            currentAction = null;
        }
        if (action != null)
        {
            if (!currentArea != action.area)
            {
                MoveToArea(action.area);
            }
            currentAction = action;
            Stage.AddResourceValue(ResourceType.money, -currentAction.StartMoneyCost);
        }
        SpecialistBar.instance.RefreshList();
    }
    public void StopAction()
    {
        SetAction(null);
    }
    /// <summary>
    /// 指令专家移动到目标地点
    /// </summary>
    /// <param name="area"></param>
    public void MoveToArea(Area area)
    {
        currentArea = area;
    }
    /// <summary>
    /// 获取当前位置说明
    /// </summary>
    /// <returns></returns>
    public string GetCurrentAreaName()
    {
        return currentArea != null ? currentArea.areaName + "-" + currentArea.region.name : "总部";
    }
    /// <summary>
    /// 获取当前工作进度
    /// </summary>
    /// <returns></returns>
    public float GetActionProgressRate()
    {
        return HasAction ? currentAction.ProgressRate : 1;
    }
    /// <summary>
    /// 获取等级
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// 获取累计经验值
    /// </summary>
    /// <returns></returns>
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
        if (level == GetMaxLevel())
            return 1.0f;
        int prevExpCap = expCaps[level - 1];
        return (exp - prevExpCap) / (expCaps[level] - prevExpCap);
    }
    /// <summary>
    /// 获取最高等级
    /// </summary>
    /// <returns></returns>
    public static int GetMaxLevel()
    {
        return expCaps.Length - 1;
    }
    /// <summary>
    /// 获取指定专长等级，如果没有会返回0
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    public int GetAbilityLevel(Ability ability)
    {
        return abilities.ContainsKey(ability) ? abilities[ability] : 0;
    }
    /// <summary>
    /// 增加经验值
    /// </summary>
    /// <param name="value"></param>
    public void AddExp(int value)
    {
        if (exp + value > GetExpCap()) //level up
        {
            string levelUpMessage = "在 " + currentArea.areaName + " 工作的 " + name + " 获得了经验";
            Ability ability = EnumHelper.GetRandomValue<Ability>();
            int oldLevel = GetAbilityLevel(ability);
            int increase = addSpeciality_randomRange_getIncrease(EnumHelper.GetRandomValue<Ability>(), 2, 2);
            if (increase > 0)
            {
                levelUpMessage += "\n" + AbilityDescription.GetAbilityDescription(ability) + ": " + oldLevel + " -> " + GetAbilityLevel(ability);
            }
            PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow("专家升级", levelUpMessage));
        }
        exp += value;
    }
}
