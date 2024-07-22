using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionProcessor : EnemyProcessor
{
    private Patrol patrolScript;
    private EnemyFOV fovScript;
    public AIPath aiPath;
    public bool inPatrol;
    public bool inPursuit;
    public Transform player; // Assuming you have a reference to the player Transform
    public float patrolSpeed = 3;
    public float runningSpeed = 5;
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
            SwitchToWalking();
            aiPath.maxSpeed = patrolSpeed;
            GetComponent<AIDestinationSetter>().enabled = true;
            GetComponent<AIDestinationSetter>().target = player; // Assuming you have an AIDestinationSetter script
        }
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
                SwitchToRunning();
                aiPath.maxSpeed = runningSpeed;
                GetComponent<AIDestinationSetter>().enabled = true;
                GetComponent<AIDestinationSetter>().target = player;
            }
        }
    }
}
