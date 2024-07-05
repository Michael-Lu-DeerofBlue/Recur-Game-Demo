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

    private enum SkillType { Attack, Bleed, Flee }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                break;
            case SkillType.Bleed:
                Attack(BleedBlockDamage);
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
        }
        else if (randomValue < attackProbability + BleedBlockProbability)
        {
            SkillCastingTime = BleedBlockCastingTime;  
            nextSkill = SkillType.Bleed;
        }
        else
        {
            SkillCastingTime = FleeCastingTime;
            nextSkill = SkillType.Flee;
        }
        nextMove = nextSkill.ToString();
    }
}
