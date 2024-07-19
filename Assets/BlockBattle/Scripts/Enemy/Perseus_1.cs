using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Perseus_1 : Enemy
{
    public int attackDamage = 3;
    public int CurseOfGorgonDamage = 80;

    public int attackWeight = 1;
    public int CurseOfGorgonWeight = 1;

    public float attackCastingTime = 8;
    public float CurseOfGorgonCastingTime = 60;

    private enum SkillType { Attack, CurseOfGorgon }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.CurseOfGorgon:
                DealAttackDamage(CurseOfGorgonDamage);
                soundManager.PlaySfx("Perseus_Gorgon");
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + CurseOfGorgonWeight;
        float attackProbability = (float)attackWeight / sum;
        float CurseOfGorgonProbability = (float)CurseOfGorgonWeight / sum;
        float randomValue = Random.value;


        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            CurrentSkillIcons = new string[] { "Damage" };
            NextSkillDamage = attackDamage;

        }
        else
        {
            SkillCastingTime = CurseOfGorgonCastingTime;  // CorruptingWind action
            nextSkill = SkillType.CurseOfGorgon;
            CurrentSkillIcons = new string[] { "Damage" };

        }
        nextMove = nextSkill.ToString();
    }
}