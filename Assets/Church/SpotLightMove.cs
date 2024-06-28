using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpotLightMove : MonoBehaviour
{
    public WaypointManager waypointManager; // Reference to the WaypointManager
    public float rotationSpeed = 1.0f; // Speed at which the spotlight rotates
    public float waitTime = 2.0f; // Time to wait before selecting a new target
    public float intensityChangeSpeed = 1.0f; // Speed at which the intensity changes
    public float maxIntensity = 1.0f; // Maximum intensity of the spotlight

    private Light spotlight;
    private Transform currentTarget;
    public bool spotlightOn;
    public GameObject spotlightManager;
    private bool first = true;
    void Start()
    {
        waypointManager = GetComponent<WaypointManager>();
        spotlight = GetComponent<Light>();
        if (spotlight == null)
        {
            Debug.LogError("No Light component found on this GameObject.");
            return;
        }

        if (waypointManager != null && waypointManager.waypointTransforms.Count > 0)
        {
            currentTarget = waypointManager.waypointTransforms[0]; // Start with the first waypoint
            //StartCoroutine(RotateToTargets());
        }
    }

    IEnumerator RotateToTargets()
    {
        if (first) { currentTarget = waypointManager.waypointTransforms[0]; first = false; }

        while (spotlightOn)
        {
            //Debug.Log("Here");
            
            // Open the spotlight (increase intensity)
            yield return StartCoroutine(ChangeIntensity(maxIntensity, intensityChangeSpeed));

            // Rotate towards the current target
            yield return StartCoroutine(RotateTowardsTarget(currentTarget));

            // Wait for the specified time
            yield return new WaitForSeconds(waitTime);

            // Close the spotlight (decrease intensity)
            yield return StartCoroutine(ChangeIntensity(0, intensityChangeSpeed));
            // Choose the next target from the connected waypoints
            var connectedTargets = waypointManager.GetConnectedTargets(currentTarget);
            if (connectedTargets.Count > 0)
            {
                currentTarget = connectedTargets[Random.Range(0, connectedTargets.Count)];
            }
            spotlightOn = false;
            spotlightManager.GetComponent<SpotlightManager>().TurnOffSpotlight(gameObject);
        }
    }

    IEnumerator ChangeIntensity(float targetIntensity, float speed)
    {
        if (spotlight != null)
        {
            while (!Mathf.Approximately(spotlight.intensity, targetIntensity))
            {
                spotlight.intensity = Mathf.MoveTowards(spotlight.intensity, targetIntensity, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    IEnumerator RotateTowardsTarget(Transform target)
    {
        if (target == null) yield break;

        while (true)
        {
            // Determine the direction to the target
            Vector3 direction = target.position - transform.position;
            //direction.x = 0; // Lock the rotation on the z-axis

            // Calculate the desired rotation towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Lock the z-axis rotation
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0);

            // Smoothly rotate towards the target at a constant speed
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if the rotation is complete
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void TurnMeOn()
    {
        spotlightOn = true;
        StartCoroutine(RotateToTargets());
    }

}
