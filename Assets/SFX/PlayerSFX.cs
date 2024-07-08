using UnityEngine;
using Fungus;

public class PlayerSFX : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] GameObject player;
    public Flowchart flowchart; // Reference to the Fungus Flowchart
    private bool wasWalking = false; // Track the previous walking state
    private bool wasSprinting = false; // Track the previous sprinting state

    void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController reference is missing.");
            return;
        }
        
        // Get player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        bool inAir = playerController.inAir;
        bool isGrounded = playerController.isGrounded;
        bool inSprint = playerController.inSprint;

        // Check if player is walking
        bool isWalking = (moveHorizontal != 0 || moveVertical != 0) && !inAir && isGrounded && !inSprint;

        // Check if player is sprinting
        bool isSprinting = (moveHorizontal != 0 || moveVertical != 0) && !inAir && isGrounded && inSprint;

        if (isWalking && !wasWalking)
        {
            // Player has started walking
            PlayEffectWalk();
        }
        else if (!isWalking && wasWalking)
        {
            // Player has stopped walking
            StopEffectWalk();
        }

        if (isSprinting && !wasSprinting)
        {
            // Player has started sprinting
            PlayEffectSprint();
        }
        else if (!isSprinting && wasSprinting)
        {
            // Player has stopped sprinting
            StopEffectSprint();
        }

        // Update the previous movement states
        wasWalking = isWalking;
        wasSprinting = isSprinting;
    }

    void PlayEffectWalk()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart for walking
            flowchart.ExecuteBlock("PlayerWalk");
        }
    }

    void StopEffectWalk()
    {
        // Stop the Fungus flowchart to stop the walking sound
        flowchart.ExecuteBlock("StopPlayerWalk"); 
    }

    void PlayEffectSprint()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart for sprinting
            flowchart.ExecuteBlock("PlayerSprint");
        }
    }

    void StopEffectSprint()
    {
        // Stop the Fungus flowchart to stop the sprinting sound
        flowchart.ExecuteBlock("StopPlayerSprint");
    }
}