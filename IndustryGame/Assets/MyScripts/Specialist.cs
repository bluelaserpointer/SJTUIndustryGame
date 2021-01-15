using System.Collections.Generic;
using UnityEngine;

public class Specialist
{
    public SpecialistTemplate specialistTemplate;
    int hireCost;
    Dictionary<Ability, int> abilities = new Dictionary<Ability, int>();

    Area currentArea;
    Action currentAction;
}
