using System;
using System.Collections.Generic;
using UnityEngine;

public static class ListLogic
{
    public static T GetRandomElement<T>(List<T> array)
    {
        int dmpIndex;
        return GetRandomElement(array, out dmpIndex);
    }
    public static T GetRandomElement<T>(List<T> array, out int index)
    {
        int areasCount = array.Count;
        if(areasCount > 0)
        {
            index = UnityEngine.Random.Range(0, areasCount);
            return array[index];
        }
        else
        {
            index = -1;
            return default(T);
        }
    }
    public static List<T> GetUniqueRandomElements<T>(List<T> array, Func<T, List<T>, bool> continueCondition)
    {
        List<T> lestArray = new List<T>(array); 
        List<T> resultArray = new List<T>();
        while (lestArray.Count > 0)
        {
            int index;
            T pickedElement = GetRandomElement(lestArray, out index);
            lestArray.RemoveAt(index);
            resultArray.Add(pickedElement);
            if (!continueCondition.Invoke(pickedElement, resultArray))
                break;
        }
        return resultArray;
    }
    public static List<T> GetUniqueRandomElements<T>(List<T> array, int amount)
    {
        return amount <= 0 ? new List<T>() : GetUniqueRandomElements(array, (pickedElement, resultArray) => resultArray.Count < amount);
    }
}
