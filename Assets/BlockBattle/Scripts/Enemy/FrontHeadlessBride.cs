using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadlessFront : Enemy
{
    public Sprite[] SculptureGlaneAnim;
    public Sprite[] SculptureGazeAnim;
    public int attackDamage = 3; 
    public int SculptureGlaneDamage = 2;
    public int SculptureGazeDamage = 2;


    public int attackWeight = 2;
    public int SculptureGlaneWeight = 2;
    public int SculptureGazeWeight = 1;
    
    public float attackCastingTime = 8;
    public float SculptureGlaneCastingTime = 4;
    public float SculptureGazeCastingTime = 10;

    private enum SkillType { Attack, SculptureGlane, SculptureGaze }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f);
                break;
            case SkillType.SculptureGlane:
                Attack(SculptureGlaneDamage);
                battleManager.AddStunBlock(1,8);
                break;
            case SkillType.SculptureGaze:
                Attack(SculptureGazeDamage);
                battleManager.AddStunBlock(3, 8);
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + SculptureGlaneWeight + SculptureGazeWeight;
        float attackProbability = (float)attackWeight / sum;
        float SculptureGlaneProbability = (float)SculptureGlaneWeight / sum;
        float randomValue = Random.value;
        

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            CurrentSkillIcons = new string[] { "Damage" };

        }
        else if (randomValue < attackProbability + SculptureGlaneProbability)
        {
            SkillCastingTime = SculptureGlaneCastingTime;  
            nextSkill = SkillType.SculptureGlane;
            CurrentSkillIcons = new string[] { "Damage" , "Interrupt"};

        }
        else
        {
            SkillCastingTime = SculptureGazeCastingTime;
            nextSkill = SkillType.SculptureGaze;
            CurrentSkillIcons = new string[] { "Damage", "Interrupt" };

        }
        nextMove = nextSkill.ToString();
    }
}
