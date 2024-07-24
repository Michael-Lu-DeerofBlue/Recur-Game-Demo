using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerProcessor : EnemyProcessor
{
    private Patrol patrolScript;
    private EnemyFOV fovScript;
    public AIPath aiPath;
    public bool inPatrol;
    public bool inPursuit;
    public bool animated;
    public Transform player; // Assuming you have a reference to the player Transform
    public float patrolSpeed = 5;
    public float runningSpeed = 7;
    // Start is called before the first frame update

    private void Start()
    {
        fovScript = GetComponent<EnemyFOV>();
    }
    public void RounteCheck()
    {
        GraphNode currentNode = AstarPath.active.GetNearest(transform.position).node;
        GraphNode playerNode = AstarPath.active.GetNearest(player.position).node;
        if (PathUtilities.IsPathPossible(currentNode, playerNode))
        {
            if (!animated)
            {
                SwitchToWalking();
                animated = true;
            }
            aiPath.maxSpeed = patrolSpeed;
            GetComponent<AIDestinationSetter>().enabled = true;
            GetComponent<AIDestinationSetter>().target = player; // Assuming you have an AIDestinationSetter script
            inPatrol = true;
        }
    }

    private void FixedUpdate()
    {
        RounteCheck();
    }

    // Update is called once per frame
    void Update()
    {
        if (fovScript.canSeePlayer)
        {
            GraphNode currentNode = AstarPath.active.GetNearest(transform.position).node;
            GraphNode playerNode = AstarPath.active.GetNearest(player.position).node;
            if (PathUtilities.IsPathPossible(currentNode, playerNode))
            {
                if (inPatrol)
                {
                    SwitchToRunning();
                }
                inPatrol = false;
                aiPath.maxSpeed = runningSpeed;
                GetComponent<AIDestinationSetter>().enabled = true;
                GetComponent<AIDestinationSetter>().target = player;
            }
        }
    }
   
}
