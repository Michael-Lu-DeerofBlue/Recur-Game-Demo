using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for 3D enemies. Provides common functionality such as locking rotation and roaming behavior.
/// </summary>
public abstract class ThreeDEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Initialization code can be added here if needed
    }

    // Update is called once per frame
    void Update()
    {
        // Per-frame logic can be added here if needed
    }

    /// <summary>
    /// Locks the enemy's rotation on the x and z axes, allowing rotation only on the y-axis.
    /// </summary>
    public void RotationLock()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// Basic roaming behavior that also locks the enemy's rotation on the x and z axes.
    /// </summary>
    public void Roam()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        // Additional roaming logic can be added here
    }
}
