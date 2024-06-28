using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Revolover: HeroInfo
{
    public int BulletNum=0;
    public TextMeshPro BulletNumber;
    public TextMeshPro CounterRemindTime;
    public float CounterTimer = 0;
    public bool CounterReady = false;
    public override void Start()
    {
        base.Start();
        BulletNumber.text = "BulletNumber: " + BulletNum.ToString();
        CounterRemindTime.text = "Counter Remind Time: " + CounterTimer.ToString();
    }
    public override void Update()
    {
        base.Update();
        if (CounterReady&& !battleManager.TimeStop)
        {
            CounterTimer -= Time.deltaTime;
            UpdateUI();
            if (CounterTimer <= 0)
            {
                CounterReady = false;
                CounterTimer = 0;
            }
        }
    }

    public override void CheckLandOn(int ColorIndex)
    {
        if (ColorIndex == 5)
        {
            CounterReady = true;
            CounterTimer = 0.5f;
            UpdateUI();
            Debug.Log("Counter Ready");
        }
    }


    public override void HitHandle(float damage)
    {
        if (CounterReady)
        {
            AttackEnemy(16);
            Debug.Log("Counter sucess!!!!!");
            return;
        }
        HitPoint -= damage;
        Hp.text = "HP: " + Mathf.RoundToInt(HitPoint).ToString();
        Debug.Log("Player is hit. HP: " + HitPoint);
        if (HitPoint <= 0)
        {
            HitPoint = 0;
            Debug.Log("Player is dead.");
        }
    }



    public void UpdateUI()
    {
        BulletNumber.text = "BulletNumber: " + BulletNum.ToString();
        CounterRemindTime.text = "Counter Remind Time: " + CounterTimer.ToString();
    }

    public void Reload(int Num)
    {
        BulletNum += Num;
        if (BulletNum > 6)
        {
            BulletNum = 6;
        }
        UpdateUI();
    }
    public void BulletCostingSkill(int BulletCost, float attackvalue)
    {
        if (BulletNum >= BulletCost)
        {
            BulletNum -= BulletCost;
            AttackEnemy(attackvalue);
        }
        else
        {
            Debug.Log("Not enough bullet");
        }
        UpdateUI();
    }

    public void FastFanning(int DamageRate)
    {
        if(BulletNum >= 6)
        {
            BulletNum -= 6;
            AttackEnemy(5 * DamageRate);
        }
        else
        {
            AttackEnemy(5 * BulletNum);
            BulletNum = 0;
        }
        UpdateUI();
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
                AttackEnemy(1);
                Fragile(1);
                break;
            case 2:
                AttackEnemy(1);
                Fragile(1);
                break;
            case 3:
                AttackEnemy(3);
                Fragile(3);
                break;
            case 4:
                AttackEnemy(4);
                Fragile(4);
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
                BulletCostingSkill(2, 16);
                break;
            case 2:
                BulletCostingSkill(2, 16);
                break;
            case 3:
                BulletCostingSkill(2, 23);
                break;
            case 4:
                BulletCostingSkill(2, 23);
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
                FastFanning(7);
                break;
            case 2:
                FastFanning(7); 
                break;
            case 3:
                FastFanning(7); 
                break;
            case 4:
                FastFanning(8);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }
    }
}
