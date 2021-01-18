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
    private Dictionary<Ability, List<Specialist>> specialists = new Dictionary<Ability, List<Specialist>>();

    public void Awake()
    {
        if (instance == null)
            instance = this;
        //init specialists
        for(int i = 0; i < (int)Ability.End; ++i)
        {
            specialists.Add((Ability)i, new List<Specialist>());
        }
    }
    public static void refresh()
    {
        System.Random random = new System.Random();
        foreach (KeyValuePair<Ability, List<Specialist>> specialityAndSpecialist in instance.specialists)
        {
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
                specialityAndSpecialist.Value.Add(specialist);
            }
        }
        //iteration print sample
        /*
        foreach(KeyValuePair<Ability, List<Specialist>> keyValuePair in instance.specialists)
        {
            Ability ability = keyValuePair.Key;
            foreach(Specialist specialist in keyValuePair.Value)
            {
                InGameLog.AddLog(AbilityDescription.GetAbilityDescription(ability) + " $" + specialist.hireCost);
            }
        }
        */
    }
    public static List<Specialist> getSpecialists(Ability specality)
    {
        return instance.specialists[specality];
    }
    public static void hireSpecialist(Ability specality, int indexInList)
    {
        List<Specialist> list = instance.specialists[specality];
        if (indexInList < list.Count) {
            Specialist specialist = list[indexInList];
            Stage.GetSpecialists().Add(specialist);
            specialist.moveToArea(Stage.getBaseArea()); //spawn in basement
            list.RemoveAt(indexInList);
            Stage.subMoney(specialist.hireCost);
        }
    }
}
