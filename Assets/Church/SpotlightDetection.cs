using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightDetection : MonoBehaviour
{
    public Light spotlight; // Reference to the spotlight
    public Transform[] playerSpots;
    public Transform player; // Reference to the player
    public string playerLayerName = "Player"; // Name of the player layer
    public string wallLayerName = "Wall"; // Name of the wall layer

    private int playerLayer;
    private int wallLayer;
    private LayerMask layerMask;
    void Start()
    {
        // Get the layer indices from the layer names
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        wallLayer = LayerMask.NameToLayer(wallLayerName);
        // Combine the layers into a single layerMask
        layerMask = (1 << playerLayer) | (1 << wallLayer);
    }
    void Update()
    {
        //bool playerInSpotLight = PlayerInSpotLight();
    }

    public bool PlayerInSpotLight()
    {
        if (spotlight.intensity > 800)
        {
            foreach (Transform spot in playerSpots)
            {
                Vector3 directionToPlayer = spot.position - spotlight.transform.position;
                //Debug.Log(spot.transform.name);
                // Check if the player is within the spotlight's range
                if (directionToPlayer.magnitude <= spotlight.range)
                {
                    //Debug.Log("In Range");
                    // Check if the player is within the spotlight's angle
                    float angle = Vector3.Angle(spotlight.transform.forward, directionToPlayer);
                    if (angle <= spotlight.spotAngle / 2)
                    {
                        //Debug.Log("In angle");
                        // Perform a raycast to see if the player is obstructed
                        RaycastHit hit;
                        if (Physics.Raycast(spotlight.transform.position, directionToPlayer, out hit, spotlight.range, layerMask))
                        {
                            //Debug.Log("Hit");
                            // Check if the raycast hit the player
                            //Debug.Log(hit.transform.name);
                            if (hit.transform == player)
                            {
                                //Debug.Log("Player is being shined on." + Time.time);
                                return true;
                                // Implement your logic for when the player is shined on
                            }
                            else
                            {
                                //Debug.Log("Player is in shadow." + Time.time);
                                // Implement your logic for when the player is in shadow
                            }
                        }
                    }
                }
            }
        }
        
        //Debug.Log("Player is in shadow." + Time.time);
        return false;
    }
}