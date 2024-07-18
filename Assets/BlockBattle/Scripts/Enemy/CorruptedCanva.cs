using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CorruptedCanva : Enemy
{
    public int attackDamage = 2;
    public int corruptingWindDamage = 2;
    public int attackWeight = 2;
    public int paintingSplashWeight = 1;
    public int corruptingWindWeight = 1;
    public float attackCastingTime = 8;
    public float splashCastingTime = 10;
    public float corruptingWindCastingTime = 10;    
    private enum SkillType { Attack, PaintingSplash, CorruptingWind }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {

        //RefreshChoiceSectionBlock(); //这个交给你了！！！！！！！！！！！！！！！！！！！！
        switch (nextSkill)
        {
            case SkillType.Attack:
                Attack(attackDamage);
                break;
            case SkillType.PaintingSplash:
                DropDownblock(3);
                break;
            case SkillType.CorruptingWind:
                Attack(corruptingWindDamage);
                LockRotationForNextBlock();
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + paintingSplashWeight + corruptingWindWeight;
        float attackProbability = (float)attackWeight / sum;
        float paintingSplashProbability = (float)paintingSplashWeight / sum;
        float corruptingWindProbability = (float)corruptingWindWeight / sum;
        float randomValue = Random.value;
        

        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            NextSkillDamage= attackDamage;
        }
        else if (randomValue < attackProbability + paintingSplashProbability)
        {
            SkillCastingTime = splashCastingTime;  // PaintingSplash action
            nextSkill = SkillType.PaintingSplash;
        }
        else
        {
            SkillCastingTime = corruptingWindCastingTime;  // CorruptingWind action
            nextSkill = SkillType.CorruptingWind;
        }
        nextMove = nextSkill.ToString();
    }
}
