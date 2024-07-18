using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class deer : Enemy
{

    public int attackDamage = 10; 
    public int ChargeDamage = 0;
    public int GoldenAntlerDamage = 12;


    public int attackWeight = 25;
    public int ChargeWeight = 35;
    public int GoldenAntlerWeight = 40;
    public int flee = 0;
    
    public float attackCastingTime = 8;
    public float ChargeCastingTime = 4;
    public float GoldenAntlerCastingTime = 10;

    private enum SkillType { Attack, Charge, GoldenAntler,Flee }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f);
                break;
            case SkillType.Charge:
                battleManager.ExcitAllEnemies(20);
                break;
            case SkillType.GoldenAntler:
               Attack(GoldenAntlerDamage);
               Enemy[] enemies = FindObjectsOfType<Enemy>();
              foreach (Enemy enemy in enemies)
                {
                    Heal(GoldenAntlerDamage / 2); 
                }
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + ChargeWeight+GoldenAntlerWeight;
        float attackProbability = (float)attackWeight / sum;
        float ChargeProbability = (float)ChargeWeight / sum;
        float randomValue = Random.value;
        

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            CurrentSkillIcons = new string[] { "Damage" };
            NextSkillDamage= attackDamage;

        }
        else if (randomValue < attackProbability + ChargeProbability)
        {
            SkillCastingTime = ChargeCastingTime;  
            nextSkill = SkillType.Charge;
            CurrentSkillIcons = new string[] { "Damage" , "Buff"};

        }
        else
        {
            SkillCastingTime = GoldenAntlerCastingTime;
            nextSkill = SkillType.GoldenAntler;
            CurrentSkillIcons = new string[] { "Damage", "Heal" };

        }
        nextMove = nextSkill.ToString();
    }
}
