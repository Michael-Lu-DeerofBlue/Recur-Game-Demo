using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightMove : MonoBehaviour
{
    public List<Transform> targets; // List of potential targets
    public float rotationSpeed = 1.0f; // Speed at which the spotlight rotates
    public float waitTime = 2.0f; // Time to wait before selecting a new target
    public float intensityChangeSpeed = 1.0f; // Speed at which the intensity changes
    public float maxIntensity = 1.0f; // Maximum intensity of the spotlight

    private Light spotlight;
    private Transform currentTarget;

    void Start()
    {
        spotlight = GetComponent<Light>();
        if (spotlight == null)
        {
            Debug.LogError("No Light component found on this GameObject.");
            return;
        }

        if (targets.Count > 0)
        {
            StartCoroutine(RotateToTargets());
        }
    }

    IEnumerator RotateToTargets()
    {
        while (true)
        {
            // Select a random target from the list
            currentTarget = targets[Random.Range(0, targets.Count)];

            // Open the spotlight (increase intensity)
            yield return StartCoroutine(ChangeIntensity(maxIntensity, intensityChangeSpeed));

            // Rotate towards the selected target
            yield return StartCoroutine(RotateTowardsTarget(currentTarget));

            // Close the spotlight (decrease intensity)
            yield return StartCoroutine(ChangeIntensity(0, intensityChangeSpeed));

            // Wait for the specified time
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator ChangeIntensity(float targetIntensity, float speed)
    {
        while (!Mathf.Approximately(spotlight.intensity, targetIntensity))
        {
            spotlight.intensity = Mathf.MoveTowards(spotlight.intensity, targetIntensity, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RotateTowardsTarget(Transform target)
    {
        if (target == null) yield break;

        while (true)
        {
            // Determine the direction to the target
            Vector3 direction = target.position - transform.position;

            // Calculate the desired rotation towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);

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

}
