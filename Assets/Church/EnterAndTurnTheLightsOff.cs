using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterAndTurnTheLightsOff : MonoBehaviour
{
    public Color ambientColor = Color.white;
    public float duration = 5.0f; // Duration over which the light will decrease
    public GameObject LevelController;
    public void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            LevelController.GetComponent<Level2>().ColdStart(); 
        }
    }

    
}
