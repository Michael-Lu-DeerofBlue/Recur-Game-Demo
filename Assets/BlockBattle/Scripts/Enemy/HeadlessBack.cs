using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadlessBack : Enemy
{
    public int attackDamage = 3;
    public int VagueSightDamage = 2;

    public int attackWeight = 2;
    public int VagueSightWeight = 2;

    public float attackCastingTime = 8;
    public float VagueSightCastingTime = 4;

    private enum SkillType { Attack, VagueSight }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                break;
            case SkillType.VagueSight:
                Attack(VagueSightDamage);
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + VagueSightWeight;
        float attackProbability = (float)attackWeight / sum;
        float VagueSightProbability = (float)VagueSightWeight / sum;
        float randomValue = Random.value;


        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
        }
        else
        {
            SkillCastingTime = VagueSightCastingTime;  // CorruptingWind action
            nextSkill = SkillType.VagueSight;
        }
        nextMove = nextSkill.ToString();
    }
}
