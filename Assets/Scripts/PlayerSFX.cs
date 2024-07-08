using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class PlayerSFX : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] GameObject player;
    public Flowchart flowchart; // Reference to the Fungus Flowchart
    private bool wasWalking = false; // Track the previous movement state

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

        //float movementSpeed = playerController.movementSpeed;
        bool inAir = playerController.inAir;
        bool isGrounded = playerController.isGrounded;

        // Check if player is walking
        bool isWalking = (moveHorizontal != 0 || moveVertical != 0) && !inAir && isGrounded;

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

        // Update the previous movement state
        wasWalking = isWalking;
    }

    void PlayEffectWalk()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("PlayerWalk"); // Replace "PlayerWalk" with the name of your block
        }
    }

    void StopEffectWalk()
    {
        // Stop the Fungus flowchart or any other logic to stop the walking sound
        // For example, you can call another block in Fungus to stop the sound
        flowchart.ExecuteBlock("StopPlayerWalk"); // Replace "StopPlayerWalk" with the name of your block that stops the sound
    }
}
