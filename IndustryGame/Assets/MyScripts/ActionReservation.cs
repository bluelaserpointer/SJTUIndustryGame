using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - Reservation")]
public class ActionReservation : AreaAction
{
    public override void actionEffect(Area area)
    {
        area.addReservation();
    }
}
