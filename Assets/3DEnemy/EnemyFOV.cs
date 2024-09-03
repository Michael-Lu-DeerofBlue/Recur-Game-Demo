using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyFOV : MonoBehaviour
{
    // Public variables to define the field of view properties
    public float radius;   // Detection radius
    [Range(0, 360)]
    public float angle;    // Field of view angle

    public GameObject playerRef;  // Reference to the player GameObject

    // Layers to define what the enemy can detect and what blocks detection
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;  // Flag to check if the enemy can see the player

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");  // Find player at start
        StartCoroutine(FOVRoutine());  // Start the FOV check routine
    }

    private void OnEnable()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");  // Find player when enabled
        StartCoroutine(FOVRoutine());  // Start the FOV check routine
    }

    /// <summary>
    /// Coroutine that repeatedly checks the field of view at set intervals.
    /// </summary>
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);  // Wait time between checks

        while (true)
        {
            yield return wait;  // Wait for the specified time
            FieldOfViewCheck();  // Perform the FOV check

            // If the player is in sight, trigger pursuit behavior
            if (canSeePlayer)
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName == "church_with_code")
                {
                    gameObject.GetComponent<ThreeEnemyBase>().inPursuit(playerRef.transform);
                }
            }
        }
    }

    /// <summary>
    /// Checks if the player is within the enemy's field of view and updates canSeePlayer.
    /// </summary>
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);  // Check for targets within radius

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Check if the target is within the angle of view
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Check if there are no obstructions between the enemy and the target
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;  // Player is in sight
                }
                else
                {
                    canSeePlayer = false;  // Player is blocked by an obstruction
                }
            }
            else
            {
                canSeePlayer = false;  // Player is outside the field of view
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;  // No targets in range, reset canSeePlayer
        }
    }
}
