using UnityEngine;
using Fungus;
using Pathfinding;

public class EnemySFX : MonoBehaviour
{
    public AIPath aiPath; // Reference to the AIPath script
    public Flowchart flowchart; // Reference to the Fungus Flowchart

    private bool wasWalking = false; // Track the previous walking state
    private bool wasSprinting = false; // Track the previous sprinting state
    private Vector3 lastPosition; // Track the last position to calculate velocity

   void Awake()
    {
        aiPath = GetComponent<AIPath>();
    }

    void Start()
    {
        lastPosition = transform.position; // Initialize the last position
    }
    
    void Update()
    {
        if (aiPath == null)
        {
            Debug.LogError("AIPath reference is missing.");
            return;
        }

        // Calculate the velocity based on position change
        Vector3 currentPosition = transform.position;
        Vector3 velocity = (currentPosition - lastPosition) / Time.deltaTime;

        // Ignore vertical (y-axis) movement
        velocity.y = 0;

        lastPosition = currentPosition;

        // Determine if the enemy is walking
        bool isWalking = velocity.magnitude > 0 && !aiPath.isStopped && !aiPath.reachedEndOfPath;

        if (isWalking && !wasWalking)
        {
            // Enemy has started walking
            PlayEffectWalk();
        }
        else if (!isWalking && wasWalking)
        {
            // Enemy has stopped walking
            StopEffectWalk();
        }

        // Update the previous movement state
        wasWalking = isWalking;
    }

    void PlayEffectWalk()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart for walking
            flowchart.ExecuteBlock("EnemyWalk");
        }
    }

    void StopEffectWalk()
    {
        // Stop the Fungus flowchart for walking
        flowchart.ExecuteBlock("StopEnemyWalk");
    }
}