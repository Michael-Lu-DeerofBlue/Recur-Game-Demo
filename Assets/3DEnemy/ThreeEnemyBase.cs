using IndieMarc.EnemyVision;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Unity.VisualScripting;

public class ThreeEnemyBase : MonoBehaviour
{
    // Public variables
    public GameObject Player;
    public bool inPursuitBool;
    public float inPursuitDelayTime;
    public float stoolDelayTime;
    public float materialDelayTime;
    public bool inStool;
    public Transform[] enemySpots;
    public Material originalMaterial;
    public Material newMaterial;
    public Transform model;
    public bool inChange;
    public bool hasKey;
    public float patrolSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");  // Initialize the player reference
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if the enemy is in pursuit mode
        if (inPursuitBool)
        {
            float distance = Vector3.Distance(gameObject.transform.position, Player.transform.position);
            if (distance < 2f)
            {
                // Stop pursuit and trigger player hit behavior
                inPursuitBool = false;
                Player.GetComponent<ThreeDPlayerBase>().gotHitByEnemy();
                Stool();
            }
        }
    }

    // FixedUpdate is called at fixed intervals (currently unused)
    void FixedUpdate()
    {
    }

    /// <summary>
    /// Starts a timer to check if the enemy should stop pursuing the player.
    /// </summary>
    IEnumerator TimerStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stop pursuit if the player is no longer visible
        if (!gameObject.GetComponent<EnemyFOV>().canSeePlayer && inPursuitBool)
        {
            backToPatrol();
        }
        else
        {
            StartCoroutine(TimerStart(inPursuitDelayTime));  // Restart timer if still in pursuit
        }
    }

    /// <summary>
    /// Starts a timer to make the enemy stay on the stool for a specified duration.
    /// </summary>
    IEnumerator StoolTimerStart(float delay)
    {
        gameObject.GetComponent<Patrol>().enabled = false;
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        inStool = true;
        yield return new WaitForSeconds(delay);
        backToPatrol();
        inStool = false;
    }

    /// <summary>
    /// Starts a timer to revert the enemy's material after a specified duration.
    /// </summary>
    IEnumerator MaterialTimerStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        Renderer renderer = model.GetComponent<Renderer>();
        renderer.material = originalMaterial;
        inChange = false;
    }

    /// <summary>
    /// Starts a timer before returning to patrol mode.
    /// </summary>
    IEnumerator BackToPatroTimerStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        gameObject.GetComponent<Patrol>().enabled = true;
    }

    /// <summary>
    /// Initiates pursuit behavior when the player is detected.
    /// </summary>
    public void inPursuit(Transform target)
    {
        if (!inStool)
        {
            inPursuitBool = true;
            gameObject.GetComponent<Patrol>().enabled = false;
            gameObject.GetComponent<AIDestinationSetter>().target = Player.transform;
            gameObject.GetComponent<AIDestinationSetter>().enabled = true;
            gameObject.GetComponent<EnemyFOV>().radius = 12;
            StartCoroutine(TimerStart(inPursuitDelayTime));
        }
    }

    /// <summary>
    /// Handles the enemy's response to a bell event, directing it to a target.
    /// </summary>
    public void inBell(Transform target)
    {
        inStool = false;
        inPursuitBool = true;
        gameObject.GetComponent<Patrol>().enabled = false;
        gameObject.GetComponent<AIDestinationSetter>().target = target;
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        gameObject.GetComponent<EnemyFOV>().radius = 12;
        StartCoroutine(TimerStart(10));
    }

    /// <summary>
    /// Returns the enemy to patrol mode after pursuing the player.
    /// </summary>
    public void backToPatrol()
    {
        inPursuitBool = false;
        gameObject.GetComponent<AIPath>().maxSpeed = patrolSpeed;
        int i = gameObject.GetComponent<Patrol>().targets.Length;
        int r = Random.Range(0, i);
        gameObject.GetComponent<AIDestinationSetter>().target = gameObject.GetComponent<Patrol>().targets[r];
        StartCoroutine(BackToPatroTimerStart(1));
        gameObject.GetComponent<EnemyFOV>().radius = 6;
    }

    /// <summary>
    /// Puts the enemy in stool mode, disabling patrol and destination setting temporarily.
    /// </summary>
    public void Stool()
    {
        StartCoroutine(StoolTimerStart(stoolDelayTime));
    }

    /// <summary>
    /// Changes the enemy's material and then reverts it after a delay if the enemy has the key.
    /// </summary>
    public void ChangeMaterial()
    {
        if (!inChange && hasKey)
        {
            inChange = true;
            Renderer renderer = model.GetComponent<Renderer>();
            renderer.material = newMaterial;
            StartCoroutine(MaterialTimerStart(materialDelayTime));
        }
    }
}
