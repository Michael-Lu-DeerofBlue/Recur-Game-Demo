using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroInfo : MonoBehaviour
{
    public int HitPoint;
    public int MaxHitPoint;
    public int AttackValue;
    private float MaxWeight;
    public string WeaponType;
    public TextMeshPro Hp;
    // Start is called before the first frame update
    void Start()
    {
        Hp.text = "HP: " + HitPoint.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExecuteBehavior(int colorCode, int clearNumber)
    {
        switch (colorCode)
        {
            case 0:
                LightAttack(clearNumber);
                //Debug.Log("Executing Red Behavior, Clear " + clearNumber);
                break;
            case 1:
                //Debug.Log("Executing Yellow Behavior, Clear " + clearNumber);
                break;
            case 2:
                Heal(clearNumber);
                //Debug.Log("Executing Green Behavior, Clear " + clearNumber);
                break;
            case 3:
                //Debug.Log("Executing Purple Behavior, Clear " + clearNumber);
                break;
            case 4:
                HeavyAttack(clearNumber);
                //Debug.Log("Executing Blue Behavior, Clear " + clearNumber);
                break;
            case 5:
                SpUlt(clearNumber);
                //Debug.Log("Executing Light Blue Behavior, Clear " + clearNumber);
                break;
            case 6:
                //Debug.Log("Executing Dark Blue Behavior, Clear " + clearNumber);
                break;
            default:
                //Debug.Log("Unknown behavior, Clear " + clearNumber);
                break;
        }
        
    }

    public void LightAttack(int clearNumber)
    {
        int value;
        switch (clearNumber)
        {
            case 1:
                value = 1;
                break;
            case 2:
                value = 2;
                break;
            case 3:
                value = 3;
                break;
            case 4:
                value = 5;
                break;
            default:
                value = 0;
                break;
        }
        AttackEnemy(value);
    }

    public void HeavyAttack(int clearNumber)
    {
        int value;
        switch (clearNumber)
        {
            case 1:
                value = 3;
                break;
            case 2:
                value = 4;
                break;
            case 3:
                value = 5;
                break;
            case 4:
                value = 8;
                break;
            default:
                value = 0;
                break;
        }
        AttackEnemy(value);
    }

    public void SpUlt(int clearNumber)
    {
        int value;
        switch (clearNumber)
        {
            case 1:
                value = 1;
                break;
            case 2:
                value = 1;
                break;
            case 3:
                value = 1;
                break;
            case 4:
                value = 10;
                break;
            default:
                value = 0;
                break;
        }
        AttackEnemy(value);
    }

    public void Heal(int clearNumber)
    {
        int value;
        switch (clearNumber)
        {
            case 1:
                value = 2;
                break;
            case 2:
                value = 3;
                break;
            case 3:
                value = 4;
                break;
            case 4:
                value = 5;
                break;
            default:
                value = 0;
                break;
        }
        HitHandle(-value);
    }



    public void HitHandle(int damage)
    {
        HitPoint -= damage;
        Hp.text = "HP: " + HitPoint.ToString();
        Debug.Log("Player is hit. HP: " + HitPoint);
        if (HitPoint <= 0)
        {
            HitPoint = 0;
            Debug.Log("Player is dead.");
        }
    }

    void AttackEnemy(int value)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 1)
        {
            BaseEnemy enemy = enemies[0].GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.HitHandle(value);
            }
        }
        else if (enemies.Length > 1)
        {
            Debug.Log("muti enemies function will be added later.");
        }
    }
}