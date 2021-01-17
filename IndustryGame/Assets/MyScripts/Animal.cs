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
    [Serializable]
    public struct SpeciesAffect
    {
        public Animal target;
        public int change;
    }
    public List<SpeciesAffect> speciesAffects;

    private int migrateLimit = 10;

    public void idle(Area area, int amount)
    {
        if (amount == 0)
            return;
        //check food requirements satisfied for how many units

        handleAnimalCount(area, amount);
    }

    public void handleAnimalCount(Area area, int amount)
    {
        // 1. 无 “不满足情况”
        // dislikeness >= 0: 直接可迁徙（部分或整体）、无需迁徙，越大越严重
        // dislikeness < 0: 迁徙, 越小越严重
        int dislikeness = getAreaDislikeness(area, amount); // migrationNum, dislikeness
        
        Area migrateArea = null; // destination
        List<Area> neighbors = area.GetNeighborAreas();

        int finalMigrationNum = 0;

        if(dislikeness >= 0) // 当前地区可养殖动物，effects 生效
        {
            int magnitude = amount - dislikeness; // comfortLevel 多，需要迁徙的越多
            foreach (SpeciesAffect affect in speciesAffects)
            {
                area.changeSpeciesAmount(affect.target, affect.change * magnitude); // 可养殖动物数的 effects 生效
            }
            
            if(dislikeness > migrateLimit) // 迁徙流量限制, 判断是否需要迁徙, dislikeness: migrationNum
                finalMigrationNum = migrateLimit;
            else
                finalMigrationNum = dislikeness;

        }
        else if(dislikeness < 0) // 当前地区不可养殖动物，直接迁徙，无视 effects, dislikeness: dislikeness,   -9, -1
            finalMigrationNum = migrateLimit;

        migrateArea = pickMigrateArea(neighbors, finalMigrationNum);

        if(migrateArea != null)
        {
            migrate(area, migrateArea, finalMigrationNum);
        }
    }

    public void migrate(Area src, Area dst, int migrationNum)
    {
        src.changeSpeciesAmount(this, src.getSpeciesAmount(this) - migrationNum);
        dst.changeSpeciesAmount(this, dst.getSpeciesAmount(this) + migrationNum);
    }

    public Area pickMigrateArea(List<Area> neighbors, int amount)
    {
        // Iterate through the areas

        // Filter out the unmigratable neigbors, and find migrate areas in migratable neighbors

        int bestHabitat = -100;
        int bestNonHabitat = 1000000;
        Area bestHabitatArea = null; // 部分动物满足
        Area bestNonHabitatArea = null; // 完全不满足
        
        foreach(Area area in neighbors)
        {
            // dislikeness > 0, habitat
            // dislikeness < 0, nonHabitat

            int dislikeness = getAreaDislikeness(area, amount);
            if(dislikeness > 0) // migrationNum
            {
                // find smallest
                // 1*, 2, 3, 5
                if(bestHabitat > dislikeness)
                {
                    bestHabitatArea = area;
                    bestHabitat = dislikeness;
                }
            }
            else // dislikeness
            {
                 // find largest
                 // -9, -5, -1*
                if(bestNonHabitat < dislikeness)
                {
                    bestNonHabitatArea = area;
                    bestNonHabitat = dislikeness;
                }
            }
        }



        // return resultMigration;
    }

    public float getAreaDislikeness(Area area, int amount)
    {
        float dislikeNess = -0f; // Score for sorting the best area for migration(间接迁徙意图)

        int satisfiedAmount = amount;

        foreach (SpeciesAffect affect in speciesAffects)
        {
            if(affect.change < 0) //minus change means food requirements
            {
                int tmpSatisfiedAmount = area.getSpeciesAmount(affect.target) % (-affect.change); // target num is smaller than one unit of effect

                if(dislikeNess < 0)
                {
                    // current Area, 3 effect 无法生效， 和当前animal的匹配程度很低
                    if(tmpSatisfiedAmount == 0) // sum the effects that don't match
                    {
                        dislikeNess -= 1; // dislikeNess = -3
                    }
                }else{
                    if(tmpSatisfiedAmount < satisfiedAmount) //take least magnitude
                    {
                        satisfiedAmount = tmpSatisfiedAmount;
                        if (satisfiedAmount == 0) // If one of the conditions fail, migrate and won't go inside this if statement
                        {
                            dislikeNess -= 1;
                        }
                    }
                }

                float satisfiedRatio = tmpSatisfiedAmount / satisfiedAmount;
            }
        }

        migrationNum = amount - satisfiedAmount;

        return dislikeNess;
    }
}
