using System.Collections.Generic;
using UnityEngine;

public class SpecialistEmployList : MonoBehaviour {
    public List<SpecialistTemplate> indoorSpecialistTemplates;
    public List<SpecialistTemplate> outdoorSpecialistTemplates;
    private Dictionary<Ability, LinkedList<Specialist>> specialists = new Dictionary<Ability, LinkedList<Specialist>>();
    public void refresh()
    {
        System.Random random = new System.Random();
        foreach(KeyValuePair<Ability, LinkedList<Specialist>> specialityAndSpecialist in specialists) {
            Ability speciality = specialityAndSpecialist.Key;
            specialityAndSpecialist.Value.Clear();
            for(int i = 0; i < 3; ++i)
            {
                Specialist specialist = new Specialist();
                specialist.name = "randomName";
                specialist.birthday = "randomBirthDay";
                int abilityLevelTotal = 0;
                if (random.NextDouble() < 0.5f) { //indoor
                    specialist.specialistTemplate = indoorSpecialistTemplates[random.Next(0, indoorSpecialistTemplates.Count)];
                    abilityLevelTotal += specialist.addSpeciality_range(speciality, 7, 10);
                    if (random.NextDouble() < 0.1f)
                    {
                        int level2 = random.Next(2, 4); // 1 ~ 3
                        specialist.abilities.Add(speciality, level2);
                        abilityLevelTotal += level2;
                    }
                } else { //outdoor
                    specialist.specialistTemplate = outdoorSpecialistTemplates[random.Next(0, outdoorSpecialistTemplates.Count)];
                }
                specialist.hireCost = abilityLevelTotal * 20;
            }
        }
    }
    public LinkedList<Specialist> getSpecialists(Ability specality)
    {
        return specialists[specality];
    }
}
