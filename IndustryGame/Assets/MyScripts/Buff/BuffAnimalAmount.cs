using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Buff - AnimalAmount")]
public class BuffAnimalAmount : AreaBuff
{
    public Animal animal;
    public int change;
    public override void Applied(Area area, float power)
    {
    }

    public override void Idle(Area area, float power)
    {
        area.changeSpeciesAmount(animal, (int)(change * power));
    }

    public override void Removed(Area area, float power)
    {
    }
}
