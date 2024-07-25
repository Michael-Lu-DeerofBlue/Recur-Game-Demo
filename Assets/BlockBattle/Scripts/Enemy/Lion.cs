using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lion : Enemy
{
    public int attackDamage = 2; 
    public int BleedBlockDamage = 7;
    public int FleeDamage = 0;


    public int attackWeight = 1;
    public int BleedBlockWeight = 1;
    public int FleeWeight = 0;
    
    public float attackCastingTime = 8;
    public float BleedBlockCastingTime = 4;
    public float FleeCastingTime = 10;

    private enum SkillType { Attack, Bite, Flee }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.Bite:
                soundManager.PlaySfx("LionBite");
                DealAttackDamage(BleedBlockDamage);
                AddBleeding();
                break;
            case SkillType.Flee:
                deadhandle();
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + BleedBlockWeight;
        float attackProbability = (float)attackWeight / sum;
        float BleedBlockProbability = (float)BleedBlockWeight / sum;
        float randomValue = Random.value;
        

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            CurrentSkillIcons = new string[] { "Damage" };
            NextSkillDamage = attackDamage;
        }
        else if (randomValue < attackProbability + BleedBlockProbability)
        {
            SkillCastingTime = BleedBlockCastingTime;  
            nextSkill = SkillType.Bite;
            CurrentSkillIcons = new string[] { "Damage", "Interrupt" };
        }
        else
        {
            SkillCastingTime = FleeCastingTime;
            nextSkill = SkillType.Flee;
            CurrentSkillIcons = new string[] { "Speical" };

        }
        nextMove = nextSkill.ToString();
    }
}

