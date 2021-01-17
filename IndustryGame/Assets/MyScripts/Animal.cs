using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Animal")]
public class Animal : ScriptableObject
{
    public string animalName;
    [TextArea]
    public string description;
    public EnvironmentType environment;
    public GameObject model;
    public int growSpeed;
    List<Animal> foods;
    public int energyNeeds;
    public int energyAsFood;
    public double minTempreture, maxTempreture;

    public int migrateLimit = 10;

    public void idle(Area area, int amount)
    {
        if (amount == 0)
            return;
        //check food requirements satisfied for how many units
        handleAnimalCount(area, amount);
    }

    public void handleAnimalCount(Area area, int amount)
    {
        double dislikeness = 0.0;
        // dislikeness: 0.0 ~ 1.0
        // reason for increase: foods, tempreture

        //grow
        int foodSatisfiedAmount = area.ProvideFood(foods, energyNeeds, amount);
        area.changeSpeciesAmount(this, foodSatisfiedAmount * growSpeed);
        dislikeness += (double)foodSatisfiedAmount / amount;

        //TODO: tempreture dislikeness affection

        //decide how many should migrate to other area
        int migrationAmount = (int)(amount * dislikeness);
        if (migrationAmount > migrateLimit)
        {
            migrationAmount = migrateLimit;
        }
        if (migrationAmount > 0) // do migration
        {
            Area migrateDst = null;
            double leastMigrateDstDislikeness = 1.0;
            foreach (Area areaIteration in area.GetNeighborAreas())
            {
                double newDislikeness = getAreaDislikeness(areaIteration);
                if (newDislikeness < leastMigrateDstDislikeness)
                {
                    migrateDst = areaIteration;
                    leastMigrateDstDislikeness = newDislikeness;
                }
            }
            if(migrateDst != null)
            {
                migrate(area, migrateDst, migrationAmount);
            }
        }
    }

    public void migrate(Area src, Area dst, int migrationAmount)
    {
        src.changeSpeciesAmount(this, -migrationAmount);
        dst.changeSpeciesAmount(this, +migrationAmount);
    }

    public double getAreaDislikeness(Area area) //TODO
    {
        double dislikeness = 0.0;
        return dislikeness;
    }
}
