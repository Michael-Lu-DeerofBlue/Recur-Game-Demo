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
    public Light pointLight;
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
            StartCoroutine(ChangeChildrenIntensity(15f, 1));

            // Rotate towards the current target
            yield return StartCoroutine(RotateTowardsTarget(currentTarget));

            // Wait for the specified time
            yield return new WaitForSeconds(waitTime);

            // Close the spotlight (decrease intensity)
            yield return StartCoroutine(ChangeIntensity(0, intensityChangeSpeed));
            StartCoroutine(ChangeChildrenIntensity(0f, 1));
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

    IEnumerator ChangeChildrenIntensity(float targetIntensity, float speed)
    {
            if (pointLight != null && pointLight.type == LightType.Point)
            {
                float initialIntensity = pointLight.intensity;
                float elapsedTime = 0f;

                while (elapsedTime < (targetIntensity - initialIntensity) / speed)
                {
                    pointLight.intensity = Mathf.MoveTowards(pointLight.intensity, targetIntensity, speed * Time.deltaTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                pointLight.intensity = targetIntensity;
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

    public void Pause()
    {
        // Save the current rotation
        ES3.Save(gameObject.name + "spotlightRotation", transform.rotation);

        // Save the current target's position
        if (currentTarget != null)
        {
            ES3.Save(gameObject.name + "currentTargetPosition", currentTarget.position);
        }
        Debug.Log("i'm saved");
    }

    public void Resume()
    {
        // Load the saved rotation
        if (ES3.KeyExists(gameObject.name + "spotlightRotation"))
        {
            transform.rotation = ES3.Load<Quaternion>(gameObject.name + "spotlightRotation");
        }

        // Load the saved target position
        if (ES3.KeyExists(gameObject.name + "currentTargetPosition"))
        {
            Vector3 targetPosition = ES3.Load<Vector3>(gameObject.name + "currentTargetPosition");
            foreach (Transform waypoint in waypointManager.waypointTransforms)
            {
                if (waypoint.position == targetPosition)
                {
                    currentTarget = waypoint;
                    break;
                }
            }
        }

        // Resume the rotation coroutine
        TurnMeOn();
    }

    public void TurnMeOn()
    {
        spotlightOn = true;
        StartCoroutine(RotateToTargets());
    }

}
