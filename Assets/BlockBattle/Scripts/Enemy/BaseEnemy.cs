using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Make sure to include this namespace

public class BaseEnemy : MonoBehaviour
{
    public int HP;
    public int AttackValue = 2;
    public float timeToExecuteTurn;

    private float timer;
    public Text enemyInfoText;
    private GameObject hero;

    void Start()
    {
        timer = timeToExecuteTurn;

        // Find the Text component in the Canvas
        enemyInfoText = GameObject.Find("EnemyInfoText").GetComponent<Text>();
        hero = GameObject.Find("Hero");
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ExecuteTurn();

            timer = timeToExecuteTurn;
        }

        enemyInfoText.text = "HP: " + HP + "\nTime to Execute Turn: " + timer.ToString("F2");

    }

    
    void ExecuteTurn() 
    {
        AttackPlayer();
    }

    void AttackPlayer()
    {
        if (hero != null)
        {
            HeroInfo heroInfo = hero.GetComponent<HeroInfo>();
            if (heroInfo != null)
            {
                heroInfo.HitHandle(AttackValue);
            }
            else
            {
                Debug.LogError("Hero object does not have a HeroInfo component.");
            }
        }
    }


    public void HitHandle(int damage)
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
}


