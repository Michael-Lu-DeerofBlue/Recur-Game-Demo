using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MockingBird: Enemy
{
    public int attackDamage = 3; 
    public int DisableInputDamage = 4;
    public int FleeDamage = 0;


    public int attackWeight = 1;
    public int DisableInputWeight = 7;
    public int FleeWeight = 0;
    
    public float attackCastingTime = 8;
    public float DisableInputCastingTime = 4;
    public float FleeCastingTime = 10;

    private enum SkillType { Attack, Scream, Flee }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.Scream:
                string[] clipNames = { "BirdScream1", "BirdScream2", "BirdScream3" };
                int randomIndex = Random.Range(0, 2);
                string selectedClipName = clipNames[randomIndex];
                soundManager.PlaySfx(selectedClipName);
                battleManager.disableInputForSeconds(2);
                break;
            case SkillType.Flee:
                deadhandle();
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + DisableInputWeight;
        float attackProbability = (float)attackWeight / sum;
        float disableInputProbability = (float)DisableInputWeight / sum;
        float randomValue = Random.value;
        

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            NextSkillDamage= attackDamage;
        }
        else if (randomValue < attackProbability + disableInputProbability)
        {
            SkillCastingTime = DisableInputCastingTime;  
            nextSkill = SkillType.Scream;
        }
        else
        {
            SkillCastingTime = FleeCastingTime;
            nextSkill = SkillType.Flee;
        }
        nextMove = nextSkill.ToString();
    }
}
