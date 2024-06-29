using IndieMarc.EnemyVision;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Level2 : LevelController
{
    public GameObject Player;
    public List<GameObject> activeSpotlights;
    public List<Transform> enemies;
    public GameObject spotlightManager;
    public bool playerShined;
    public int randomIndex;

    // Start is called before the first frame update
    void Awake()
    {
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = true;
        if (ThreeDTo2DData.ThreeDScene != null)
        {
            ThreeDTo2DData.ThreeDScene = null;
            Reload();
        }
        else
        {
            SetRandomEnemyHasKey();
        }
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
        Save();
        base.GoToBattle();
    }

    public void Reload() //Player, Enemy, SpotLights
    {
        //Player
        if (ES3.KeyExists("InLevelPlayerPosition"))
        {
            Player.transform.position = ES3.Load<Vector3>("InLevelPlayerPosition");
        }
        if (ES3.KeyExists("InLevelPlayerRotation"))
        {
            Player.transform.rotation = ES3.Load<Quaternion>("InLevelPlayerRotation");
        }

        //Enemy
        foreach (Transform enemy in enemies)
        {
            enemy.position = ES3.Load<Vector3>(enemy.name + " Position");
            enemy.rotation = ES3.Load<Quaternion>(enemy.name + " Rotation");
        }
        ThreeEnemyBase enemyScript = enemies[randomIndex].GetComponent<ThreeEnemyBase>();
        enemyScript.hasKey = true;

        //Spotlights
        spotlightManager.GetComponent<SpotlightManager>().Resume();
    }

    public void Save()
    {
        //Player
        Player.GetComponent<PlayerController>().Save();

        //Enemy
        foreach (Transform enemy  in enemies)
        {
            ES3.Save(enemy.name + " Position", enemy.position);
            ES3.Save(enemy.name + " Rotation", enemy.rotation);
        }
        ES3.Save("EnemyKeyIndex", randomIndex);

        //Spotlights
        spotlightManager.GetComponent<SpotlightManager>().Pause();
    }

    void SetRandomEnemyHasKey()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("No enemies in the list.");
            return;
        }

        randomIndex = Random.Range(0, enemies.Count);
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
