using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FledPixel: Enemy
{
    public int attackDamage = 2;
    public int attackWeight = 2;
    public float attackCastingTime = 8;  
    private enum SkillType { Attack, PaintingSplash, CorruptingWind }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
    Attack(2);
    }

    public override void GetNextMove()
    {
        nextSkill = SkillType.Attack;
        NextSkillDamage = attackDamage;
    }
}
