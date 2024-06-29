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
            StartCoroutine(GraduallyReduceAmbientLight());
        }
    }

    IEnumerator GraduallyReduceAmbientLight()
    {
        LevelController.GetComponent<Level2>().ColdStart();
        float elapsedTime = 0.0f;
        Color initialColor = RenderSettings.ambientLight;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;

            RenderSettings.ambientLight = Color.Lerp(initialColor, ambientColor, lerpFactor);
            yield return null;
        }

        // Ensure the final color is set
        RenderSettings.ambientLight = ambientColor;
    }
}
