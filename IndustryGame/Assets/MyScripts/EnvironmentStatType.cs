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
    [Header("造成的影响")]
    public List<AreaBuff> buffs;
}

public class EnvironmentStat
{
    private readonly EnvironmentStatType type;
    public readonly Area area;

    /// <summary>
    /// 环境指标名称
    /// </summary>
    public string name { get { return type.statName; } }
    /// <summary>
    /// 环境指标说明
    /// </summary>
    public string description { get { return type.description; } }
    /// <summary>
    /// 造成的影响
    /// </summary>
    public List<AreaBuff> buffs { get { return type.buffs; } }
    /// <summary>
    /// 指标值
    /// </summary>
    public float value;
    public EnvironmentStat(EnvironmentStatType environmentStatType, Area area)
    {
        type = environmentStatType;
        this.area = area;
        buffs.ForEach(buff => buff.Applied(area, value));
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
        buffs.ForEach(buff => buff.Idle(area, value));
    }
    /// <summary>
    /// 清除环境指标
    /// </summary>
    public void Removed()
    {
        buffs.ForEach(buff => buff.Removed(area, value));
    }
}