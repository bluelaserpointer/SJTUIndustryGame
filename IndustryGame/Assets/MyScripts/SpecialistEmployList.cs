using System.Collections.Generic;
using UnityEngine;

/**
 *  usage: 
 *  refresh: SpecialistEmployList.refresh();
 *  access: SpecialistEmployList.getSpecialists(Ability.***);
 *  hire: SpecialistEmployList.hireSpecialist(Specialist specialist)
 */
public static class SpecialistEmployList {
    private static SpecialistTemplate[] indoorSpecialistTemplates = Resources.LoadAll<SpecialistTemplate>("Specialist/Indoor");
    private static SpecialistTemplate[] outdoorSpecialistTemplates = Resources.LoadAll<SpecialistTemplate>("Specialist/Outdoor");
    private static Dictionary<Ability, List<Specialist>> specialists = new Dictionary<Ability, List<Specialist>>();

    static SpecialistEmployList()
    {
        for (int i = 0; i < (int)Ability.End; ++i)
        {
            specialists.Add((Ability)i, new List<Specialist>());
        }
    }
    public static void refresh()
    {
        System.Random random = new System.Random();
        foreach (KeyValuePair<Ability, List<Specialist>> specialityAndSpecialist in specialists)
        {
            Ability speciality = specialityAndSpecialist.Key;
            specialityAndSpecialist.Value.Clear();
            for(int i = 0; i < 3; ++i)
            {
                Specialist specialist = new Specialist();
                specialist.name = Resources.Load<NameTemplates>("NameTemplates/SpecialistName").pickRandomOne();
                specialist.birthday = "randomBirthDay";
                specialist.birthplace = Resources.Load<NameTemplates>("NameTemplates/CityName").pickRandomOne();
                int abilityLevelTotal = 0;
                if (random.NextDouble() < 0.5f) { //indoor
                    specialist.specialistTemplate = indoorSpecialistTemplates[random.Next(0, indoorSpecialistTemplates.Length)];
                    abilityLevelTotal += specialist.addSpeciality_range(speciality, 7, 10);
                    for(int j = 0; j < 2; ++j)
                    {
                        abilityLevelTotal += specialist.addSpeciality_range((Ability)random.Next(0, (int)Ability.End), 0, 2);
                    }
                } else { //outdoor
                    specialist.specialistTemplate = outdoorSpecialistTemplates[random.Next(0, outdoorSpecialistTemplates.Length)];
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
        foreach(KeyValuePair<Ability, List<Specialist>> keyValuePair in specialists)
        {
            Ability ability = keyValuePair.Key;
            foreach(Specialist specialist in keyValuePair.Value)
            {
                InGameLog.AddLog(AbilityDescription.GetAbilityDescription(ability) + " " + specialist.specialistTemplate.specialistType.ToString());
            }
        }
        */
    }
    public static List<Specialist> getSpecialists(Ability specality)
    {
        return specialists[specality];
    }
    public static void hireSpecialist(Specialist specialist)
    {
        //List<Specialist> list = instance.specialists[specality];
        //if (indexInList < list.Count) {
        //    Specialist specialist = list[indexInList];
        //    Stage.GetSpecialists().Add(specialist);
        //    specialist.moveToArea(Stage.getBaseArea()); //spawn in basement
        //    list.RemoveAt(indexInList);
        //    Stage.subMoney(specialist.hireCost);
        //}

        foreach (KeyValuePair<Ability, List<Specialist>> keyValuePair in specialists)
        {
            Ability ability = keyValuePair.Key;
            int index = 0;
            foreach (Specialist specialistValue in keyValuePair.Value)
            {
                if (specialistValue == specialist)
                {
                    Stage.GetSpecialists().Add(specialist);
                    specialist.moveToArea(Stage.getBaseArea()); //spawn in basement
                    keyValuePair.Value.RemoveAt(index);
                    Stage.subMoney(specialist.hireCost);
                    return;
                }
                index++;
            }
        }
    }
}
