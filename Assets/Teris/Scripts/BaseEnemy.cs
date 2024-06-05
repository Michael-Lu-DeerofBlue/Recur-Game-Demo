using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Make sure to include this namespace

public class BaseEnemy : MonoBehaviour
{
    public int HP;
    public float timeToExecuteTurn;

    private float timer;
    public Text enemyInfoText;  

    void Start()
    {
        timer = timeToExecuteTurn;

        // Find the Text component in the Canvas
        enemyInfoText = GameObject.Find("EnemyInfoText").GetComponent<Text>();
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
        Debug.Log(gameObject.name + " is executing its turn.");
    }
}
