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
}
