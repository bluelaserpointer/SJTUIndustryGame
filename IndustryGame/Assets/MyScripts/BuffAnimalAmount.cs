using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Buff - AnimalAmount")]
public class BuffAnimalAmount : Buff
{
    public Animal animal;
    public int change;
    public override void applied()
    {
    }

    public override void idle()
    {
        foreach(Area area in Stage.getAreas())
        {
            area.changeSpeciesAmount(animal, change);
        }
    }

    public override void removed()
    {
    }
}
