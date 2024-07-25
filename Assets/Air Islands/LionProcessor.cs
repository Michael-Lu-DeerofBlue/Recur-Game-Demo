using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionProcessor : EnemyProcessor
{
    public List<Transform> patrolPoints;
    public List<Transform> actualPatrolTargets = new List<Transform>();
    public Transform current; // Which is a part of the patrolPoints
    private Patrol patrolScript;
    private EnemyFOV fovScript;
    public AIPath aiPath;
    public bool inPatrol;
    public Transform player; // Assuming you have a reference to the player Transform
    public float patrolSpeed = 5;
    public float runningSpeed = 7;

    // Start is called before the first frame update
    void Start()
    {
        patrolScript = GetComponent<Patrol>();
        fovScript = GetComponent<EnemyFOV>();
    }

    public void RounteCheck()
    {
        // Clear the actualPatrolTargets list before populating it
        actualPatrolTargets.Clear();

        // Convert current Transform position to a node
        GraphNode currentNode = AstarPath.active.GetNearest(current.position).node;


        foreach (Transform patrolPoint in patrolPoints)
        {
            //Debug.Log("here1");
            // Convert each patrol point position to a node
            GraphNode patrolNode = AstarPath.active.GetNearest(patrolPoint.position).node;

            // Check if there's a path between current node and patrol node
            // Do not self check the current Node
            if (currentNode != patrolNode && PathUtilities.IsPathPossible(currentNode, patrolNode))
            {
                // Add the patrol's Transform to actualPatrolTargets
                actualPatrolTargets.Add(patrolPoint);
            }
        }

        // Add the current patrol point to the list


        // If there are valid patrol targets, enable the patrol script and set the targets
        if (actualPatrolTargets.Count > 0)
        {
            //Debug.Log("here2");
            actualPatrolTargets.Add(current);
            patrolScript.targets = actualPatrolTargets.ToArray(); // Assuming the Patrol script has a public Transform[] targets
            aiPath.maxSpeed = patrolSpeed; //change the speed in to patrol speed
            patrolScript.enabled = true;
            inPatrol = true;
            SwitchToWalking();
        }


    }

    private void FixedUpdate()
    {
        if (!inStool)
        {
            RounteCheck();
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (inPatrol)
        {
            if (fovScript.canSeePlayer)
            {
                if (inPatrol || inPursuit)
                {
                    // Test if there is a possible path in the grid map to the player
                    GraphNode currentNode = AstarPath.active.GetNearest(transform.position).node;
                    GraphNode playerNode = AstarPath.active.GetNearest(player.position).node;

                    if (PathUtilities.IsPathPossible(currentNode, playerNode))
                    {
                        inPatrol = false;
                        inPursuit = true;
                        SwitchToRunning();
                        aiPath.maxSpeed = runningSpeed;
                        patrolScript.enabled = false;
                        GetComponent<AIDestinationSetter>().enabled = true;
                        GetComponent<AIDestinationSetter>().target = player; // Assuming you have an AIDestinationSetter script
                    }
                }
            }
        }
    }
}
