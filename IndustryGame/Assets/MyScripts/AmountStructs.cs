using System.Collections.Generic;
using UnityEngine;

public struct AmountChange
{
    public float old;
    private float current;
    public float change;
    public AmountChange(float initialValue) { old = current = initialValue; change = 0; }
    public float Add(float value) { return current += value; }
    public float AddWithClamp(float value, float min, float max) { return current = Mathf.Clamp(current + value, min, max); }
    public float AddWithoutRecord(float value) { old += value; return current += value; }
    public void recordChange()
    {
        if (current < 0)
            current = 0;
        change = current - old;
        old = current;
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
        records.AddLast(new Record(amountChange.old, amountChange.change));
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