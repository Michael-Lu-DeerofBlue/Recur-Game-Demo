using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadlessBack : Enemy
{
    public int attackDamage = 3;
    public int SculptureGlanceDamage = 2;

    public int attackWeight = 2;
    public int SculptureGlanceWeight = 2;

    public float attackCastingTime = 8;
    public float SculptureGlanceCastingTime = 4;

    private enum SkillType { Attack, SculptureGlance }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                break;
            case SkillType.SculptureGlance:
                Attack(SculptureGlanceDamage);
                battleManager.AddStunBlock(1, 8);
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + SculptureGlanceWeight;
        float attackProbability = (float)attackWeight / sum;
        float VagueSightProbability = (float)SculptureGlanceWeight / sum;
        float randomValue = Random.value;


        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            CurrentSkillIcons = new string[] { "Damage" };

        }
        else
        {
            SkillCastingTime = SculptureGlanceCastingTime;  // CorruptingWind action
            nextSkill = SkillType.SculptureGlance;
            CurrentSkillIcons = new string[] { "Damage" ,"Interrupt"};

        }
        nextMove = nextSkill.ToString();
    }
}
