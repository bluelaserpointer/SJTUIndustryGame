using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - Reservation")]
public class ActionReservation : Action
{
    public override void effect(Area area)
    {
        area.addReservation();
    }
    public override bool requireArea()
    {
        return true;
    }
}
