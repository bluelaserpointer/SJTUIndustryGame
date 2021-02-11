using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ReorderableListBase<T>
{
    [SerializeReference]
    public List<T> List;
}