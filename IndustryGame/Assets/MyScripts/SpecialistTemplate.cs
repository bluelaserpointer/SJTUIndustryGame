using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/SpecialistTemplate")]
public class SpecialistTemplate : ScriptableObject
{
    public string specialistName;
    public Ability speciality;
    public enum SpecialistType { InDoor, OutDoor }
    public SpecialistType specialistType;
    public enum Jender { Male, Female }
    public Jender jender;
    public DateTime birthday;
    public enum Location { London }
    public Location birthPlace;
    [TextArea]
    public string characterBackground;
    public GameObject icon;
}
