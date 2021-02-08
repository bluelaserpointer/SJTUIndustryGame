using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ReorderableAttribute : PropertyAttribute
{
    public readonly Type generatablesNestClass;
    public ReorderableAttribute(Type generatablesNestClass)
    {
        this.generatablesNestClass = generatablesNestClass;
    }
}
