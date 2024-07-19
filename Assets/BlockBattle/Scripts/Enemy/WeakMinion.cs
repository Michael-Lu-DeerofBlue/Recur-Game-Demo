using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeakMinion : Enemy
{
    public int attackDamage = 1;
    public int CompanionsMinDamage = 1;
    public int CompanionsMaxDamage = 8;
    public int attackWeight = 1;
    public int CompanionsWeight = 1;
    public float attackCastingTime = 8;
    public float CompanionsCastingTime = 12;
    
    private enum SkillType { Attack, Companions }
    private SkillType nextSkill;
    public bool waiting = false;


    public override void ExecuteTurn()//all enemy will get casting time before they spend skill.
    {
        ExecuteSkill();
        if (waiting)
        {
            PauseCasting = true;
            return;
        }
        else 
        { 
            PauseCasting = false;
            GetNextMove();
            timer = SkillCastingTime;
        }
    }
    public void DoCampanions()
    {
        if (waiting)
        {
            RandomDamageAttack(CompanionsMinDamage, CompanionsMaxDamage);
            AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f,attackDamage);
            GetNextMove();
            timer = SkillCastingTime;
            PauseCasting = false;
            waiting = false;
        }
    }



    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                soundManager.PlaySfx("Attack_Mondrinion");
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f,attackDamage);
                break;
            case SkillType.Companions:
                string selectedClipName = Random.value > 0.5f ? "MondrinionAlt" : "Mondrinion";
                soundManager.PlaySfx(selectedClipName);
                waiting = true;
                battleManager.CheckCompanions();
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + CompanionsWeight;
        float attackProbability = (float)attackWeight / sum;
        float CompanionsProbability = (float)CompanionsWeight / sum;
        float randomValue = Random.value;


        if (battleManager.WeakMinionCompanionsOnHold)
        {
            SkillCastingTime =CompanionsCastingTime;
            nextSkill = SkillType.Companions;
        }
            else if
            (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            NextSkillDamage = Mathf.RoundToInt(attackDamage);
        }
        else 
        {
            SkillCastingTime = CompanionsCastingTime;  
            nextSkill = SkillType.Companions;
            battleManager.WeakMinoCompanionsStart();
        }
        nextMove = nextSkill.ToString();
    }
}
