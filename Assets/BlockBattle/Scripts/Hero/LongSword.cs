using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongSword : HeroInfo
{

    public override void HandleIndex0(int clearNumber) // 0: Red
    {
        switch (clearNumber)
        {
            case 1:
                
                AttackEnemy(1);
                break;
            case 2:
                AttackEnemy(2);
                break;
            case 3:
                AttackEnemy(3);
                PauseEnemyActionBar(2);
                break;
            case 4:
                AttackEnemy(4);
                PauseEnemyActionBar(3);
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
                HitHandle(2);
                break;
            case 2:
                HitHandle(3);
                break;
            case 3:
                HitHandle(4);
                break;
            case 4:
                HitHandle(5);
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
                AttackEnemy(3);
                break;
            case 2:
                AttackEnemy(4);
                break;
            case 3:
                AttackEnemy(5);
                break;
            case 4:
                AttackEnemy(8);
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
                Debug.Log("remove debuff will add in future");
                break;
            case 2:
                Debug.Log("remove debuff will add in future");
                break;
            case 3:
                Debug.Log("remove debuff will add in future");
                break;
            case 4:
                Debug.Log("remove debuff will add in future");
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
                AttackEnemy(14);
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
