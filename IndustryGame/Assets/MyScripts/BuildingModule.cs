using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - BuildingModule")]
public class BuildingModule : AreaAction
{
    public Building targetBuilding;

    public override void actionEffect(Area area)
    {
        area.buildingModules.Add(this);
    }
    public override bool enabled(Area area)
    {
        return base.enabled(area) && area.buildings.Contains(targetBuilding);
    }
}
