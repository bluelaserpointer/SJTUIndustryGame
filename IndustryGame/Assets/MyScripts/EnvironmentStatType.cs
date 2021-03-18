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
    [Header("指标值区间(min, max))")]
    public Vector2 valueRange;
    [Header("指标初始值区间(min, max)")]
    public Vector2 initialValueRange;
    [Header("指标值每日变化")]
    public float dayValueChange;
    [Header("适居性影响倍率")]
    public float habitabilityAffectRate;
    [Header("达到最大/最小值时是否消失")]
    public bool removeWhenReachMax;
    public bool removeWhenReachMin;
    [Header("专家措施每日对因素的变化(0则无法干涉)")]
    public float dayValueChangeBySpecialistAction;

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
    public string Name { get { return type.statName; } }
    /// <summary>
    /// 环境指标说明
    /// </summary>
    public string Description { get { return type.description; } }
    /// <summary>
    /// 指标值每日变化
    /// </summary>
    public float DayAffectChange { get { return type.dayValueChange; } }
    /// <summary>
    /// 专家措施对因素的影响(0则无法干涉)
    /// </summary>
    public float DayValueChangeBySpecialistAction { get { return type.dayValueChangeBySpecialistAction; } }
    /// <summary>
    /// 适居性影响倍率
    /// </summary>
    public float HabitalityAffectRate { get { return type.habitabilityAffectRate; } }
    private float factorValue;
    /// <summary>
    /// 指标值
    /// </summary>
    public float FactorValue {
        get
        {
            return factorValue;
        }
        set
        {
            factorValue = Mathf.Clamp(value, type.valueRange.x, type.valueRange.y);
        }
    }
    private bool isRevealed;
    /// <summary>
    /// 是否已发现
    /// </summary>
    public bool IsRevealed { get { return isRevealed; } }
    private bool isDestroied;
    /// <summary>
    /// 是否已被清除
    /// </summary>
    public bool IsDestroied { get { return isDestroied; } }
    public string TooltipDescription {
        get
        {
            return Name + " : " + factorValue.ToString("#0.0");
        }
    }
    public EnvironmentStatFactor(EnvironmentStatType environmentStatType, Area area)
    {
        type = environmentStatType;
        this.area = area;
        factorValue = UnityEngine.Random.Range(type.initialValueRange.x, type.initialValueRange.y);
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
        factorValue += DayAffectChange;
        if(ShouldRemove())
        {
            Remove();
        }
    }
    public bool ShouldRemove()
    {
        return type.removeWhenReachMin && factorValue <= type.valueRange.x
            || type.removeWhenReachMax && factorValue >= type.valueRange.y;
    }
    public float ReceiveAffect(int areaDistance)
    {
        float effect = SeekAffect(areaDistance);
        totalHabitabilityAffect += effect;
        return effect;
    }
    public float SeekAffect(int areaDistance)
    {
        return factorValue * type.habitabilityAffectRate / (areaDistance + 1);
    }
    /// <summary>
    /// 清除环境指标
    /// </summary>
    public void Remove()
    {
        area.environmentStatFactors.Remove(this);
        isDestroied = true;
    }
}