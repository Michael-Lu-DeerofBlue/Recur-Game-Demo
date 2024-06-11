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
    public TextMeshPro Hp;
    BattleManager battleManager;
    // Start is called before the first frame update
    void Start()
    {
       
        battleManager = FindObjectOfType<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void ExecuteBehavior(int index, int clearNumber)
    {
        switch (index)
        {
            case 0:
                HandleIndex0(clearNumber);
                break;
            case 1:
                HandleIndex1(clearNumber);
                break;
            case 2:
                HandleIndex2(clearNumber);
                break;
            case 3:
                HandleIndex3(clearNumber);
                break;
            case 4:
                HandleIndex4(clearNumber);
                break;
            case 5:
                HandleIndex5(clearNumber);
                break;
            case 6:
                HandleIndex6(clearNumber);
                break;
        }
    }

    public virtual void HitHandle(int damage)
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

    public virtual void AttackEnemy(int value)
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

    public virtual void Heal(int number)
    {
        HitPoint += number;
        Hp.text = "HP: " + HitPoint.ToString();
        Debug.Log("Player is healed. HP: " + HitPoint);
    }
    public virtual void PauseEnemyActionBar(float Delay)
    {
        battleManager.PuaseEnemyActionBar(Delay);
    }

    public virtual void HandleIndex0(int clearNumber)
    {

    }
    public virtual void HandleIndex1(int clearNumber)
    {

    }
    public virtual void HandleIndex2(int clearNumber)
    {

    }
    public virtual void HandleIndex3(int clearNumber)
    {

    }
    public virtual void HandleIndex4(int clearNumber)
    {

    }
    public virtual void HandleIndex5(int clearNumber)
    {

    }
    public virtual void HandleIndex6(int clearNumber)
    {

    }
}