using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    public int HP;
    public int AttackValue;
    public float timeToExecuteTurn;
    public Text enemyInfoText;
    public float timer;
    public GameObject hero;// Start is called before the first frame update
    private BattleManager battleManager;

    public void Start()
    {
        enemyInfoText = GameObject.Find("EnemyInfoText").GetComponent<Text>();
        hero = GameObject.Find("Hero");
        battleManager = FindObjectOfType<BattleManager>();
        GetNextMove();//get casting time for the first turn.
    }
    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ExecuteTurn();

            timer = timeToExecuteTurn;
        }

        enemyInfoText.text = "HP: " + HP + "\nTime to Execute Turn: " + timer.ToString("F2");

    }

    public virtual void ExecuteTurn()//all enemy will get casting time before they spend skill.
    {
        ExecuteSkill();
        GetNextMove();
    }

    public virtual void ExecuteSkill()
    {

    }
    public virtual void GetNextMove()
    {
    }

    public void Attack(int Damage)
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


    public virtual void HitHandle(int damage)
    {
        HP -= damage;
        Debug.Log("Enemy is hit. HP: " + HP);
        if (HP <= 0)
        {
            HP = 0;
            Debug.Log("Enemy is dead.");
            Destroy(gameObject);  // Destroy the enemy object if HP is zero or less
        }
    }
    public void RefreshChoiceSectionBlock()
    {
        // Access the BattleManager instance and set LockRotation to true
        battleManager.refreshSelectionBlocks();
    }


}
