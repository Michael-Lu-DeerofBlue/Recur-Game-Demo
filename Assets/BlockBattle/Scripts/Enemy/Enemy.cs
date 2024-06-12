using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    // Parent Class of enemy character
    public float HP;
    public float MaxHp;
    public float SkillCastingTime;
    public TextMeshPro enemyInfoText;
    public float timer;
    public GameObject hero;
    public BattleManager battleManager;
    public bool PauseCasting = false; 
    public string nextMove;//name of the skill that will be executed next.

    public void Start()
    {       
        hero = GameObject.Find("Hero");
        battleManager = FindObjectOfType<BattleManager>();
        GetNextMove();//get casting time for the first turn.
        timer = SkillCastingTime; //added this so that the first move is executed with a timer
    }
    public virtual void Update()
    {
        if (PauseCasting)//the casting time will not decrease sometime because of debug, or during interruption of block game.
        {
            return;
        }
        if(battleManager.PauseBlockGame==false)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            ExecuteTurn();
        }

        enemyInfoText.text = "HP: " + HP + "\nNext Move: " + nextMove + "\nTime to Execute Turn: " + timer.ToString("F2");

    }

    public virtual void ExecuteTurn()//all enemy will get casting time before they spend skill.
    {
        ExecuteSkill();
        GetNextMove();
        timer = SkillCastingTime;
        //after that, look at the update method. 
    }

    public virtual void ExecuteSkill()
    {

    }
    public virtual void GetNextMove()
    {
    }

    public virtual void ResetCasting()
    {
        // timetoExecuteTurn = skillcastingtime; view in child class
        timer = SkillCastingTime;
    }

    public void Attack(float Damage)
    {
        if (hero != null)
        {
            HeroInfo heroInfo = hero.GetComponent<HeroInfo>();
            if (heroInfo != null)
            {
                heroInfo.HitHandle(Damage);
            }
            else
            {
                Debug.LogError("Hero object does not have a HeroInfo component.");
            }
        }
    }

    public void DropDownblock(float second)
    {
        battleManager.DropDownBlock(second);
    }

    public void LockRotation()
    {
        // Access the BattleManager instance and set LockRotation to true
        battleManager.LockRotation();
    }

    public void LockRotationForNextBlock()
    {
        // Access the BattleManager instance and set LockRotation to true
        battleManager.LockRotationForNextBlock();
    }
    public void Heal(float healValue)
    {
        HitHandle(-healValue);
        if (HP >= 0)
        {
            HP = MaxHp;
        }
    }

    public virtual void HitHandle(float damage)
    {
        if (battleManager.EnemyFragiling)
        {
            HP -= damage*1.5f;
            Debug.Log("Enemy is receving 1.5 times damage. HP: " + HP);
        }
        else
        {
            HP -= damage;
            Debug.Log("Enemy is hit. HP: " + HP);
        }
        if (HP <= 0)
        {
            HP = 0;
            Debug.Log("Enemy is dead.");
            Destroy(gameObject);  // Destroy the enemy object if HP is zero or less
        }
    }
    public void RefreshChoiceSectionBlock()
    {
        battleManager.refreshSelectionBlocks();
    }
}
