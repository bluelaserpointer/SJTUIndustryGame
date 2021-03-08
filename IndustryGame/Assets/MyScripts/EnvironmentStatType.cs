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
    [Header("适居性影响比重")]
    public float weight = 1.0f ;
    [Header("影响每日最大增长比")]
    [Min(0.0f)]
    public float maxGrowRate;
    [Header("初始数值区间(min, max)")]
    public Vector2 initialValueRange;

    public static EnvironmentStatType[] GetAllTypes()
    {
        return Resources.LoadAll<EnvironmentStatType>("EnvironmentStat");
    }
}

public class EnvironmentStatFactor
{
    private readonly EnvironmentStatType type;
    public Area area;
    /// <summary>
    /// 累计动物数改变
    /// </summary>
    [HideInInspector]
    public float totalHabitationAffect;

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
    public float maxGrowRate { get { return type.maxGrowRate; } }
    /// <summary>
    /// 适居性影响比重
    /// </summary>
    public float weight { get { return type.weight; } }
    /// <summary>
    /// 指标值
    /// </summary>
    public float value;
    private bool isRevealed;
    /// <summary>
    /// 是否已发现
    /// </summary>
    public bool IsRevealed { get { return isRevealed; } }
    public EnvironmentStatFactor(EnvironmentStatType environmentStatType, Area area)
    {
        type = environmentStatType;
        this.area = area;
    }
    public void Reveal()
    {
        if (isRevealed)
            return;
        isRevealed = true;
        //change appearance on area HUD
        if (value < 0)
            area.environmentFactorMarkImage.color = Color.Lerp(Color.red, Color.white, value*2 + 1.0f);
        else
            area.environmentFactorMarkImage.color = Color.Lerp(Color.white, Color.green, value*2);
        area.environmentFactorMarkImage.gameObject.SetActive(true);
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
        if(value > 0.0f && maxGrowRate > 0)
        {
            //TODO: reference degree of people environment attention
            value *= 1.0f + UnityEngine.Random.Range(0.0f, maxGrowRate);
            if(value > 1.0f)
            {
                float overflow = value - 1.0f;
                value = 1.0f;
                //spreads to nearby area
                List<Area> neighborAreas = new List<Area>(area.GetNeighborAreas());
                neighborAreas[UnityEngine.Random.Range(0, neighborAreas.Count)].AddEnvironmentStat(type, overflow);
            }
        }
    }
    public float ReceiveAffect(int areaDistance)
    {
        float effect = value * weight / (areaDistance + 1);
        totalHabitationAffect += effect;
        return effect;
    }
    /// <summary>
    /// 清除环境指标
    /// </summary>
    public void Removed()
    {
    }
}