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
                
                AttackEnemy(2);
                break;
            case 2:
                AttackEnemy(4);
                break;
            case 3:
                AttackEnemy(7);
                PauseSingleEnemyActionBar(2);
                break;
            case 4:
                AttackEnemy(10);
                PauseSingleEnemyActionBar(3);
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
                Heal(5);
                break;
            case 2:
                Heal(8);
                break;
            case 3:
                Heal(12);
                break;
            case 4:
                Heal(15);
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
                AttackEnemy(5);
                break;
            case 2:
                AttackEnemy(8);
                break;
            case 3:
                AttackEnemy(12);
                break;
            case 4:
                AttackEnemy(15);
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
                RemoveDebuff(1);
                break;
            case 2:
                RemoveDebuff(1);
                break;
            case 3:
                RemoveDebuff(1);
                break;
            case 4:
                RemoveDebuff(2);
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
                AttackEnemy(3);
                break;
            case 2:
                AttackEnemy(5);
                break;
            case 3:
                AttackEnemy(6);
                resetEnemyActionBar();
                break;
            case 4:
                AttackEnemy(8);
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
                if (battleManager.clearedline == 4)
                {
                    AttackEnemy(30);
                }
                else AttackEnemy(1);

                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }



   
    public override void AnimIndex0(int clearNumber)
    {
        LongSwordAnimator.Play("Red", 0, 0.0f);
    }

    public override void AnimIndex1(int clearNumber)
    {
        LongSwordAnimator.Play("Green", 0, 0.0f);

    }
    public override void AnimIndex2(int clearNumber)
    {
        LongSwordAnimator.Play("Orange", 0, 0.0f);

    }
    public override void AnimIndex3(int clearNumber)
    {
        LongSwordAnimator.Play("Dblue", 0, 0.0f);

    }
    public override void AnimIndex4(int clearNumber)
    {
        LongSwordAnimator.Play("Purple", 0, 0.0f);
    }
    public override void AnimIndex5(int clearNumber)
    {
        LongSwordAnimator.Play("Yellow", 0, 0.0f);
    }
    public override void AnimIndex6(int clearNumber)
    {
        if(clearNumber==4  && battleManager.clearedline == 4)
        {
            LongSwordAnimator.Play("Lblue", 0, 0.0f);
        }
        else
        {
            LongSwordAnimator.Play("LblueWu", 0, 0.0f);


        }
    }


    public void PlayRedSFX()
    {
        soundManager.PlaySfx("Longsword_Light");
    }

    public void PlayOrangeSFX()
    {
        soundManager.PlaySfx("Longsword_Heavy_NoImpact");
    }

    public void PlayDblueSFX()
    {
        soundManager.PlaySfx("Longsword_RestoreStance");
    }

    public void PlayPurpleSFX()
    {
        soundManager.PlaySfx("Longsword_Krumphau");
    }

    public void PlayLblueSFX()
    {
        soundManager.PlaySfx("Longsword_Zornhau");
    }

    public void PlayLblueWuSFX()
    {
        soundManager.PlaySfx("Longsword_ZornhauW");
    }

}
