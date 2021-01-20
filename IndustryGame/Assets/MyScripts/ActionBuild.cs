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
            area.buildings.Add(building);
            building.applied();
        } else if (buildType == BuildType.remove)
        {
            if (area.buildings.Remove(building))
                building.removed();
        }
    }
}
