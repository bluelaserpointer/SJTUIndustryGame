using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/SpecialistTemplate")]
public class SpecialistTemplate : ScriptableObject
{
    public enum SpecialistType { InDoor, OutDoor }
    public SpecialistType specialistType;
    public enum Jender { Male, Female }
    public Jender jender;
    public Sprite icon;
}
