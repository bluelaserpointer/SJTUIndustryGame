using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 能记录增减量的数值保存器，UI上需要显示增减量时有用<para></para>
/// <see cref="RecordChange"/> - 周期性调用该函数使其记录增减量<para></para>
/// <see cref="GetRecordValue"/> - 访问本周期开头的数值，显示用<para></para>
/// <see cref="GetCurrentValue"/> - 访问最新值，处理用<para></para>
/// <see cref="GetChange"/> - 获取增减量(还未调用过<see cref="RecordChange"/>时返回0)
/// </summary>
public class AmountChange
{
    private float recordValue;
    private float currentValue;
    public float change;
    public AmountChange(float initialValue) { recordValue = currentValue = initialValue; }
    /// <summary>
    /// 增加数值，需调用<see cref="RecordChange"/>才会在<see cref="GetRecordValue"/>看到
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float Add(float value) { return currentValue += value; }
    public float Clamp(float min, float max) { return currentValue = Mathf.Clamp(currentValue, min, max); }
    /// <summary>
    /// 增加数值，但不影响增减量，<see cref="GetRecordValue"/>也能立刻看到变化
    /// </summary>
    /// <param name="value"></param>
    public void AddWithoutRecording(float value) { recordValue += value; currentValue += value; }
    /// <summary>
    /// 周期性调用该函数使其记录增减量
    /// </summary>
    public void RecordChange()
    {
        if (currentValue < 0)
            currentValue = 0;
        change = currentValue - recordValue;
        recordValue = currentValue;
    }
    /// <summary>
    /// 访问本周期开头的数值，显示用<para></para>
    /// 还未调用过<see cref="RecordChange"/>时返回初始值
    /// </summary>
    /// <returns></returns>
    public float GetRecordValue()
    {
        return recordValue;
    }
    /// <summary>
    /// 访问最新值，处理用
    /// </summary>
    /// <returns></returns>
    public float GetCurrentValue()
    {
        return currentValue;
    }
    /// <summary>
    /// 获取增减量<para></para>
    /// 还未调用过<see cref="RecordChange"/>时返回0)
    /// </summary>
    /// <returns></returns>
    public float GetChange()
    {
        return change;
    }
}
public class AmountChangeRecords
{
    public struct Record
    {
        public readonly int year, month, day;
        public readonly float amount;
        public readonly float change;
        public Record(float amount, float change)
        {
            year = Timer.GetYear();
            month = Timer.GetMonth();
            day = Timer.GetDay();
            this.amount = amount;
            this.change = change;
        }
    }
    private LinkedList<Record> records = new LinkedList<Record>();
    public AmountChangeRecords(AmountChange forFirstRecord)
    {
        AddRecord(forFirstRecord);
    }
    public void AddRecord(AmountChange amountChange)
    {
        records.AddLast(new Record(amountChange.GetRecordValue(), amountChange.change));
    }
    public LinkedList<Record> GetRecords()
    {
        return records;
    }
    public int? GetAmountInLatestRecord()
    {
        return records.Count > 0 ? (int?)records.Last.Value.amount : null;
    }
    public int? GetChangeInLatestRecord()
    {
        return records.Count > 0 ? (int?)records.Last.Value.change : null;
    }
}