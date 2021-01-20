using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Animal")]
public class Animal : ScriptableObject
{
    public string animalName;
    [TextArea]
    public string description;
    public EnvironmentType bestEnvironmentType;
    [Serializable]
    public struct EnvironmentPreference
    {
        public EnvironmentType environment;
        public double preference;
    }
    public List<EnvironmentPreference> environmentPreferences;
    public GameObject model;
    [Min(0)]
    public int reproductionSpeed;
    List<Animal> foods;
    [Min(0)]
    public int energyNeeds;
    [Min(0)]
    public int energyAsFood;
    public double minTempreture, maxTempreture;
    [Min(0)]
    public int migrateLimit;

    public AudioClipList sfxAudio;

    private List<int> dangerLimits;

    [Header("不同危险级别物种数目间隔")]
    public int dangerAmountSpan;
    void Awake() {
        int mostDangerType = EnumHelper.GetMaxEnum<SpeciesDangerType>();
        
        for(int i = 0; i <= mostDangerType; i++)
            dangerLimits[mostDangerType - i] = i * dangerAmountSpan;

    }
    public void idle(Area currentArea, int amount)
    {
        if (amount == 0)
            return;
        //grow: check food requirements satisfied for how many units
        int foodSatisfiedAmount = currentArea.ProvideFood(foods, energyNeeds, amount);
        currentArea.changeSpeciesAmount(this, foodSatisfiedAmount * reproductionSpeed);

        //decide how many should migrate to other area
        if (migrateLimit > 0)
        {
            int migrationAmount = (int)(amount * getAreaDislikeness(currentArea, amount)); // dislikeness: -inf ~ 1.0
            if (migrationAmount > 0) // do migration
            {
                if (migrationAmount > migrateLimit)
                {
                    migrationAmount = migrateLimit;
                }
                Area migrateDst = null;
                double leastMigrateDstDislikeness = 1.0;
                foreach (Area area in currentArea.GetNeighborAreas())
                {
                    double newDislikeness = getAreaDislikeness(area, migrationAmount);
                    if (newDislikeness < leastMigrateDstDislikeness)
                    {
                        migrateDst = area;
                        leastMigrateDstDislikeness = newDislikeness;
                    }
                }
                if (migrateDst != null)
                {
                    migrate(currentArea, migrateDst, migrationAmount);
                }
            }
        }
    }

    public void migrate(Area src, Area dst, int migrationAmount)
    {
        src.changeSpeciesAmount(this, -migrationAmount);
        dst.changeSpeciesAmount(this, +migrationAmount);
    }

    public double getAreaDislikeness(Area area, int amount)
    {
        double dislikeness = 0.0;
        //food affection
        if(energyNeeds > 0)
            dislikeness += 1.0 - (double)area.GetProvidableFoodEnergy(foods, energyNeeds) / (amount * energyNeeds);
        //environmentType affection
        foreach(EnvironmentPreference environmentAndPreference in environmentPreferences)
        {
            if(environmentAndPreference.environment.Equals(area.environmentType))
            {
                dislikeness -= environmentAndPreference.preference;
                break;
            }
        }
        //TODO: tempreture affection
        return dislikeness;
    }

    public SpeciesDangerType getDangerType(int amount)
    {
        int dangerType = EnumHelper.GetMaxEnum<SpeciesDangerType>();
        for(int i = 0; i < dangerLimits.Count; i++)
        {
            if(amount >= dangerLimits[i])
            {
                dangerType = i;
                break;
            }
        }
        return (SpeciesDangerType)dangerType;
    }
}
