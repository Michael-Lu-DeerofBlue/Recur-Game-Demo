using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Perseus : Enemy
{
    public int attackDamage = 8; 
    public int CurseOfGorgonDamage = 100;


    public int attackWeight = 1;
    public int CurseOfGorgonWeight = 1;
    
    public float attackCastingTime = 8;
    public float CurseOfGorgonCastingTime = 80;

    private enum SkillType { Attack, CurseOfGorgon }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                break;
            case SkillType.CurseOfGorgon:
                Attack(CurseOfGorgonDamage);
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + CurseOfGorgonWeight;
        float attackProbability = (float)attackWeight / sum;
        float randomValue = Random.value;
        

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
        }
        else
        {
            SkillCastingTime = CurseOfGorgonCastingTime;
            nextSkill = SkillType.CurseOfGorgon;
        }
        nextMove = nextSkill.ToString();
    }
}
