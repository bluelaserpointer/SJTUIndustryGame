using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - Build")]
public class ActionBuild : AreaAction
{
    public enum BuildType { add, remove }
    [Space]
    public BuildType buildType;
    public Building building;
    public override void actionEffect(Area area)
    {
        if(buildType == BuildType.add)
        {
            area.GetBuildings().Add(building);
            building.applied();
        } else if (buildType == BuildType.remove)
        {
            if (area.GetBuildings().Remove(building))
                building.removed();
        }
    }
}
