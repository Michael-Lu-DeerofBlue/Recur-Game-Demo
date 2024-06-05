using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExecuteBehavior(string behavior)
    {
        switch (behavior)
        {
            case "0010FFFF":
                Debug.Log("Executing Blue Behavior");
                // Add code for blue behavior
                break;
            case "00FF07FF":
                Debug.Log("Executing Green Behavior");
                // Add code for green behavior
                break;
            case "FF0000FF":
                Debug.Log("Executing Red Behavior");
                // Add code for red behavior
                break;
            default:
                Debug.Log("Unknown behavior");
                break;
        }
    }
}