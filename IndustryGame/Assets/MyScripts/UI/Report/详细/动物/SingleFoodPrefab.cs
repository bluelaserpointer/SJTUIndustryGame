using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleFoodPrefab : MonoBehaviour
{
    [Header("显示食物的名称的Text")]
    public Text FoodName;

    public void RefreshUI(Animal food)
    {
        FoodName.text = food.animalName;
    }
}
