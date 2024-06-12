using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StarryNight : Enemy
{
    public bool ChargingCorruption = false;

    public int attackDamage = 5;
    public int RealityCorruptionDamage = 42;

    public int attackWeight = 2;
    public int paintingSplashWeight = 2;
    public int GazeStarsWeight = 1;
    public int CorruptionWeight = 0;

    public float attackCastingTime = 8;
    public float splashCastingTime = 10;
    public float GazeStarsCastingtime = 5;
    public float corruptionCastingTime = 15;
    public bool firstCorruption = false;
    private enum SkillType { Attack, PaintingSplash, GazeStars, corruption }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                break;
            case SkillType.PaintingSplash:
                LockRotation();
                break;
            case SkillType.GazeStars:
                RefreshChoiceSectionBlock();
                break;
            case SkillType.corruption:
                Attack(RealityCorruptionDamage);
                ChargingCorruption = false;
                break;
        }
    }

    public override void GetNextMove()
    {
        if (HP < 50 && !firstCorruption)
        {
            firstCorruption = true;
            SkillCastingTime = corruptionCastingTime;  // Corruption action
            nextSkill = SkillType.corruption;
            StartChargingCorruption();
            CorruptionWeight = 1;
            nextMove = nextSkill.ToString();
            return;
        }
        int sum = attackWeight + paintingSplashWeight + GazeStarsWeight + CorruptionWeight;
        float attackProbability = (float)attackWeight / sum;
        float paintingSplashProbability = (float)paintingSplashWeight / sum;
        float GazeStarsProbability = (float)GazeStarsWeight / sum;
        float CorruptionProbability = (float)CorruptionWeight / sum;
        float randomValue = Random.value; //range : 0 ~ 1

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
        }
        else if (randomValue < attackProbability + paintingSplashProbability)
        {
            SkillCastingTime = splashCastingTime;  // PaintingSplash action
            nextSkill = SkillType.PaintingSplash;
        }
        else if (randomValue < attackProbability + paintingSplashProbability + GazeStarsProbability)
        {
            SkillCastingTime = GazeStarsCastingtime;  // GazeStars action
            nextSkill = SkillType.GazeStars;
        }
        else if (randomValue <= attackProbability + paintingSplashProbability + GazeStarsProbability + CorruptionProbability)
        {
            SkillCastingTime = corruptionCastingTime;  // Corruption action
            nextSkill = SkillType.corruption;
            StartChargingCorruption();
        }
        nextMove = nextSkill.ToString();

    }
    public override void HitHandle(int damage)
    {
        if(ChargingCorruption == true)
        {
            damage = 1;
        }
        else
        {
          HP -= damage;
        }
        Debug.Log("Enemy is hit. HP: " + HP);
        if (HP <= 0)
        {
            HP = 0;
            Debug.Log("Enemy is dead.");
            Destroy(gameObject);  // Destroy the enemy object if HP is zero or less
        }
    }
    public void StartChargingCorruption()
    {
       ChargingCorruption = true;
    }
}
