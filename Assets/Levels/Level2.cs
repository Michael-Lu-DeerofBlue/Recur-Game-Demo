using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2 : LevelController
{
    public GameObject Player;
    public List<GameObject> activeSpotlights;
    public List<Transform> enemies;
    public GameObject spotlightManager;
    public bool playerShined;

    // Start is called before the first frame update
    void Awake()
    {
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = true;
        SetRandomEnemyHasKey();
    }

    private void Update()
    {
        activeSpotlights = spotlightManager.GetComponent<SpotlightManager>().activeSpotlights;
        playerShined = false;
        foreach (GameObject spotlight in activeSpotlights)
        {
            playerShined = spotlight.GetComponent<SpotlightDetection>().PlayerInSpotLight();
            //Debug.Log(playerShined);
            if (playerShined)
            {
                ResetEnemyTarget(Player.transform);
                break;
            }
        }
        foreach (GameObject spotlight in activeSpotlights)
        {
            foreach (Transform enemy in enemies)
            {
                bool enemyShined = spotlight.GetComponent<SpotlightDetection>().EnemyInSpotLight(enemy);
                if (enemyShined)
                {
                    enemy.GetComponent<ThreeEnemyBase>().ChangeMaterial();
                }
            }
        }

    }

    void ResetEnemyTarget(Transform target)
    {
        foreach (Transform enemy in enemies)
        {
            enemy.GetComponent<ThreeEnemyBase>().inPursuit(Player.transform);
        }
    }

    void ResetEnemyBackToPatrol(Transform target)
    {
        foreach (Transform enemy in enemies)
        {
            enemy.GetComponent<AIDestinationSetter>().enabled = false;
            enemy.GetComponent<Patrol>().enabled = true;
        }
    }

    // Update is called once per frame
    public override void GoToBattle()
    {
        base.GoToBattle();
    }

    void SetRandomEnemyHasKey()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("No enemies in the list.");
            return;
        }

        int randomIndex = Random.Range(0, enemies.Count);
        ThreeEnemyBase enemyScript = enemies[randomIndex].GetComponent<ThreeEnemyBase>();

        if (enemyScript != null)
        {
            enemyScript.hasKey = true;
            Debug.Log("Enemy at index " + randomIndex + " now has the key.");
        }
        else
        {
            Debug.LogError("Enemy at index " + randomIndex + " does not have an Enemy script.");
        }
    }
}
