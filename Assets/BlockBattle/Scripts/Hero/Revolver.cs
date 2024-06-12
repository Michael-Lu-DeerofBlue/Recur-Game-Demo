using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolover: HeroInfo
{
    public int BulletNum=0;

    public void Reload(int Num)
    {
        BulletNum += Num;
    }

    public void BulletCostingSkill(int BulletCost, int attackvalue)
    {
        if (BulletNum >= BulletCost)
        {
            BulletNum -= BulletCost;
            AttackEnemy(attackvalue);
        }
    }

    public override void HandleIndex0(int clearNumber) // 0: Red
    {
        switch (clearNumber)
        {
            case 1:
                
                AttackEnemy(1);
                Reload(1);
                break;
            case 2:
                AttackEnemy(2);
                Reload(1);
                break;
            case 3:
                AttackEnemy(2);
                Reload(2);
                break;
            case 4:
                AttackEnemy(3);
                Reload(2);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex1(int clearNumber) // 1: Green
    {
        switch (clearNumber)
        {
            case 1:
                Heal(1);
                break;
            case 2:
                Heal(2);
                break;
            case 3:
                Heal(3);
                break;
            case 4:
                Heal(4);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex2(int clearNumber) // 2: orange
    {
        switch (clearNumber)
        {
            case 1:
                BulletCostingSkill(1, 5);
                break;
            case 2:
                BulletCostingSkill(1, 6);
                break;
            case 3:
                BulletCostingSkill(1, 6);
                break;
            case 4:
                BulletCostingSkill(1, 7);
                break;
            default:
                break;
        }
    }

    public override void HandleIndex3(int clearNumber) // 3: Dblue
    {
        switch (clearNumber)
        {
            case 1:
                Reload(3);
                break;
            case 2:
                Reload(4);
                break;
            case 3:
                Reload(5);
                break;
            case 4:
                Reload(6);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex4(int clearNumber) // 4: purple
    {
        switch (clearNumber)
        {
            case 1:
                AttackEnemy(2);
                break;
            case 2:
                AttackEnemy(3);
                break;
            case 3:
                AttackEnemy(4);
                break;
            case 4:
                AttackEnemy(7);
                resetEnemyActionBar();
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex5(int clearNumber) // 5: Yellow
    {
        switch (clearNumber)
        {
            case 1:
                parry(1);
                break;
            case 2:
                parry(1);
                break;
            case 3:
                parry(2);
                break;
            case 4:
                parry(2);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex6(int clearNumber) // 6: Lblue
    {
        switch (clearNumber)
        {
            case 1:
                AttackEnemy(1);
                break;
            case 2:
                AttackEnemy(1);
                break;
            case 3:
                AttackEnemy(1);
                break;
            case 4:
                Zornhauy(10);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }
}
