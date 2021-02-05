using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReorderableConditionList
{
    public struct MenuElement
    {
        public string text;
        public Func<RegionCondition> supplier;
        public MenuElement(string text, Func<RegionCondition>supplier)
        {
            this.text = text;
            this.supplier = supplier;
        }
    }
    // CAUTION WITH CONDITION CLASSES NAME CHANGE! Related stage data will be damaged.
    public static MenuElement[] menuElements = new MenuElement[] {
        new MenuElement("Region/CheckRegionAnimalCount", () => new CheckRegionAnimalCount()),
        new MenuElement("Region/CheckRegionBuildingCount", () => new CheckRegionBuildingCount()),
        new MenuElement("World/CheckTotalActionFinish", () => new CheckTotalActionFinish()),
        new MenuElement("World/CheckTotalAnimalCount", () => new CheckTotalAnimalCount())
    };

    [SerializeReference]
    public List<RegionCondition> List;

    public bool Judge(Region region)
    {
        return List.Find(condition => !condition.Judge(region)) == null;
    }
}