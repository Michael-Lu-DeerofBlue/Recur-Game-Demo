using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Daedalus1 : Enemy
{

    public int attackDamage = 10;
    public int WaxedWingsDamage = 16;
    public int WaxSPrayDamage = 15;
    public int KindledStrike = 20;

    public int attackWeight = 25;
    public int WaxedWingsWeight = 40;
    public int WaxSPrayWeight = 35;
    public int KindledStrikeWeight=0;
    public int TheLabyrinthWeight = 0;

    public float attackCastingTime = 8;
    public float WaxedWingsCastingTime = 4;
    public float WaxSPrayCastingTime = 10;
    public float kindledStrikeCastingTime = 20;
    public float ThelabyrinthCastingTime = 5;

    private enum SkillType { Attack, WaxedWings, WaxSPray, kindledStrike, Thelabyrinth }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.WaxedWings:

                break;
            case SkillType.WaxSPray:

                break;
            case SkillType.kindledStrike:

                break;
            case SkillType.Thelabyrinth:
                //TwoDto3D.
                break;

        }
    }

    //public override void GetNextMove()
    //{
    //    int sum = attackWeight + ChargeWeight + GoldenAntlerWeight;
    //    float attackProbability = (float)attackWeight / sum;
    //    float ChargeProbability = (float)ChargeWeight / sum;
    //    float randomValue = Random.value;


    //    if (randomValue < attackProbability)
    //    {
    //        SkillCastingTime = attackCastingTime;  // Attack action
    //        nextSkill = SkillType.Attack;
    //        CurrentSkillIcons = new string[] { "Damage" };
    //        NextSkillDamage = attackDamage;

    //    }
    //    else if (randomValue < attackProbability + ChargeProbability)
    //    {
    //        SkillCastingTime = ChargeCastingTime;
    //        nextSkill = SkillType.Charge;
    //        CurrentSkillIcons = new string[] { "Damage", "Buff" };

    //    }
    //    else
    //    {
    //        SkillCastingTime = GoldenAntlerCastingTime;
    //        nextSkill = SkillType.GoldenAntler;
    //        CurrentSkillIcons = new string[] { "Damage", "Heal" };

    //    }
    //    nextMove = nextSkill.ToString();
    //}
}
