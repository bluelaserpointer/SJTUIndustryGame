using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIconProvider : MonoBehaviour
{
    private static AbilityIconProvider instance;

    [Serializable]
    public struct AbilityAndIcon
    {
        public Ability ability;
        public Sprite icon;
    }
    [SerializeField]
    private List<AbilityAndIcon> icons;
    public void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public static Sprite GetAbilityIcon(Ability ability)
    {
        return instance.icons.Find(pair => pair.ability.Equals(ability)).icon;
    }
}