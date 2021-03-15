using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/EnvironmentStat")]
public class EnvironmentStatType : ScriptableObject
{
    [Header("环境指标名称")]
    public string statName;
    [Header("环境指标说明")]
    public string description;
    [Header("适居性影响值区间(min, max))")]
    public Vector2 habitabilityAffectRange;
    [Header("适居性初始数值区间(min, max)")]
    public Vector2 initialAffectRange;
    [Header("适居性每日变化比率")]
    public float dayAffectMultiply = 1;
    public bool destroyWhenReachMax;
    public bool destroyWhenReachMin;

    public static EnvironmentStatType[] GetAllTypes()
    {
        return Resources.LoadAll<EnvironmentStatType>("EnvironmentStat");
    }
}

public class EnvironmentStatFactor
{
    private readonly EnvironmentStatType type;
    public Area area;
    private float totalHabitabilityAffect;
    /// <summary>
    /// 累计动物数改变
    /// </summary>
    public float TotalHabitabilityAffect { get { return totalHabitabilityAffect; } }

    /// <summary>
    /// 环境指标名称
    /// </summary>
    public string name { get { return type.statName; } }
    /// <summary>
    /// 环境指标说明
    /// </summary>
    public string description { get { return type.description; } }
    /// <summary>
    /// 每日最大增长值
    /// </summary>
    public float dayAffectMultiply { get { return type.dayAffectMultiply; } }
    private float habitabilityAffect;
    /// <summary>
    /// 指标值
    /// </summary>
    public float HabitabilityAffect {
        get
        {
            return habitabilityAffect;
        }
        set
        {
            habitabilityAffect = Mathf.Clamp(value, type.habitabilityAffectRange.x, type.habitabilityAffectRange.y);
        }
    }
    private bool isRevealed;
    /// <summary>
    /// 是否已发现
    /// </summary>
    public bool IsRevealed { get { return isRevealed; } }
    public string TooltipDescription {
        get
        {
            return name + " 每日适居性影响: " + habitabilityAffect.ToString("#0.0");
        }
    }
    public EnvironmentStatFactor(EnvironmentStatType environmentStatType, Area area)
    {
        type = environmentStatType;
        this.area = area;
        habitabilityAffect = UnityEngine.Random.Range(type.initialAffectRange.x, type.initialAffectRange.y);
    }
    public void Reveal()
    {
        if (isRevealed)
            return;
        isRevealed = true;
    }
    /// <summary>
    /// 判断指标种类
    /// </summary>
    /// <param name="environmentStatType"></param>
    /// <returns></returns>
    public bool IsType(EnvironmentStatType environmentStatType)
    {
        return type.Equals(environmentStatType);
    }
    /// <summary>
    /// 每日流程
    /// </summary>
    public void DayIdle()
    {
        //affect current area
        //grow and spread
        if(habitabilityAffect != 0.0f)
        {
            habitabilityAffect *= dayAffectMultiply;
            //TODO: case of overflow
            /*float newValue = habitabilityAffect * (1.0f + UnityEngine.Random.Range(0.0f, maxGrowRate));
            if(newValue > 1.0f) {
                float overflow = newValue - 1.0f;
                habitabilityAffect = 1.0f;
                //randomly spreads to nearby area
                if (UnityEngine.Random.Range(0, 1) < overflow)
                {
                    List<Area> neighborAreas = new List<Area>(area.GetNeighborAreas());
                    neighborAreas[UnityEngine.Random.Range(0, neighborAreas.Count)].AddEnvironmentFactor(type);
                }
            } else
            {
                habitabilityAffect = newValue;
            }*/
        }
        if(type.destroyWhenReachMin && habitabilityAffect == type.habitabilityAffectRange.x
            || type.destroyWhenReachMax && habitabilityAffect == type.habitabilityAffectRange.y)
        {
            area.environmentStatFactors.Remove(this);
        }
    }
    public float ReceiveAffect(int areaDistance)
    {
        float effect = SeekAffect(areaDistance);
        totalHabitabilityAffect += effect;
        return effect;
    }
    public float SeekAffect(int areaDistance)
    {
        return habitabilityAffect / (areaDistance + 1);
    }
    /// <summary>
    /// 清除环境指标
    /// </summary>
    public void Removed()
    {
    }
}