using System.Collections.Generic;
using UnityEngine;

/**
 *  usage: 
 *  refresh: SpecialistEmployList.refresh();
 *  access: SpecialistEmployList.getSpecialists(Ability.***);
 *  remove: SpecialistEmployList.getSpecialists(Ability.***).Remove(...);
 */
public class SpecialistEmployList : MonoBehaviour {
    private static SpecialistEmployList instance;
    public List<SpecialistTemplate> indoorSpecialistTemplates;
    public List<SpecialistTemplate> outdoorSpecialistTemplates;
    private Dictionary<Ability, LinkedList<Specialist>> specialists = new Dictionary<Ability, LinkedList<Specialist>>();

    public void Awake()
    {
        if(instance == null)
            instance = this;
        refresh();
    }
    public static void refresh()
    {
        System.Random random = new System.Random();
        foreach(KeyValuePair<Ability, LinkedList<Specialist>> specialityAndSpecialist in instance.specialists) {
            Ability speciality = specialityAndSpecialist.Key;
            specialityAndSpecialist.Value.Clear();
            for(int i = 0; i < 3; ++i)
            {
                Specialist specialist = new Specialist();
                specialist.name = "randomName";
                specialist.birthday = "randomBirthDay";
                specialist.birthplace = "randomCity";
                int abilityLevelTotal = 0;
                if (random.NextDouble() < 0.5f) { //indoor
                    specialist.specialistTemplate = instance.indoorSpecialistTemplates[random.Next(0, instance.indoorSpecialistTemplates.Count)];
                    abilityLevelTotal += specialist.addSpeciality_range(speciality, 7, 10);
                    for(int j = 0; j < 2; ++j)
                    {
                        abilityLevelTotal += specialist.addSpeciality_range((Ability)random.Next(0, (int)Ability.End), 0, 2);
                    }
                } else { //outdoor
                    specialist.specialistTemplate = instance.outdoorSpecialistTemplates[random.Next(0, instance.outdoorSpecialistTemplates.Count)];
                    abilityLevelTotal += specialist.addSpeciality_range(speciality, 4, 6);
                    for (int j = 0; j < 4; ++j)
                    {
                        abilityLevelTotal += specialist.addSpeciality_range((Ability)random.Next(0, (int)Ability.End), 1, 3);
                    }
                }
                specialist.hireCost = abilityLevelTotal * 20; //level * 20 = cost
                specialityAndSpecialist.Value.AddLast(specialist);
            }
        }
       
    }
    public static LinkedList<Specialist> getSpecialists(Ability specality)
    {
        return instance.specialists[specality];
    }
}
