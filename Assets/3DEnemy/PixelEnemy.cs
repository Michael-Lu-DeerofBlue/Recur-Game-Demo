using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelEnemy : ThreeDEnemy
{
    // Start is called before the first frame update
    void Start()
    {
        // Initialization code can be added here if needed
    }

    // Update is called once per frame
    void Update()
    {
        // Call the base class method to lock the enemy's rotation
        base.RotationLock();
    }
}
