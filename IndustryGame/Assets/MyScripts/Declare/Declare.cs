using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Declare
{
    public readonly string name;
    public abstract string Description { get; }
    public enum Type { Mission, Support, Demand }
    public readonly Type type;

    public Declare(string name, Type type)
    {
        this.name = name;
        this.type = type;
    }

    public abstract void DayIdle();

    public static Declare PickRandomOne(Type type)
    {
        //TODO: edit
        return null;
    }
}
