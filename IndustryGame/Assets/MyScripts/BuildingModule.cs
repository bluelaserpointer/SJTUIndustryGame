using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - BuildingModule")]
public class BuildingModule : AreaAction
{
    public BuildingInfo targetBuilding;

    public override void actionEffect(Area area)
    {
    }
    public override bool enabled(Area area)
    {
        return base.enabled(area) && area.ContainsConstructedBuildingInfo(targetBuilding);
    }
}
