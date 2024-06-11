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
        RefreshChoiceSectionBlock();
        //switch (nextSkill)
        //{
        //    case SkillType.Attack:
        //        Attack(attackDamage);
        //        break;
        //    case SkillType.PaintingSplash:
        //        LockRotation();
        //        break;
        //    case SkillType.CorruptingWind:
        //        Attack(corruptingWindDamage);
        //        LockRotationForNextBlock();
        //        break;
        //}
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
            timer = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
        }
        else if (randomValue < attackProbability + paintingSplashProbability)
        {
            timer = splashCastingTime;  // PaintingSplash action
            nextSkill = SkillType.PaintingSplash;
        }
        else
        {
            timer = corruptingWindCastingTime;  // CorruptingWind action
            nextSkill = SkillType.CorruptingWind;
        }
    }
}
