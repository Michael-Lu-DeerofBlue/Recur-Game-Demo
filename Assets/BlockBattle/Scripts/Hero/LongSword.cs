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

    public override void HandleIndex2(int clearNumber) // 2: Blue
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

    public override void HandleIndex3(int clearNumber) // 3: Yellow
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

    public override void HandleIndex4(int clearNumber) // 4: Purple
    {
        switch (clearNumber)
        {
            case 1:
                // Add your logic for Index 4, clearNumber 1 here
                break;
            case 2:
                // Add your logic for Index 4, clearNumber 2 here
                break;
            case 3:
                // Add your logic for Index 4, clearNumber 3 here
                break;
            case 4:
                // Add your logic for Index 4, clearNumber 4 here
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex5(int clearNumber) // 5: Orange
    {
        switch (clearNumber)
        {
            case 1:
                // Add your logic for Index 5, clearNumber 1 here
                break;
            case 2:
                // Add your logic for Index 5, clearNumber 2 here
                break;
            case 3:
                // Add your logic for Index 5, clearNumber 3 here
                break;
            case 4:
                // Add your logic for Index 5, clearNumber 4 here
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }

    public override void HandleIndex6(int clearNumber) // 6: Dark blue
    {
        switch (clearNumber)
        {
            case 1:
                // Add your logic for Index 6, clearNumber 1 here
                break;
            case 2:
                // Add your logic for Index 6, clearNumber 2 here
                break;
            case 3:
                // Add your logic for Index 6, clearNumber 3 here
                break;
            case 4:
                // Add your logic for Index 6, clearNumber 4 here
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }
}
