using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightManager : MonoBehaviour
{
    public List<GameObject> spotlights; // List of all spotlight GameObjects
    public List<GameObject> activeSpotlights; // List of currently active spotlights
    public List<GameObject> inactiveSpotlights; // List of inactive spotlights
    public GameObject centralSpotlight;
    public int initialActiveSpotlights = 3; // Number of spotlights to activate at game start
    public float checkInterval = 1.0f; // Interval in seconds to check the number of active spotlights
    public float freezLightInterval = 5.0f;
    public float turnoffLightInterval = 5.0f;

    private void Update()
    {

    }

    void Start()
    {
        activeSpotlights = new List<GameObject>();
        inactiveSpotlights = new List<GameObject>(spotlights);

        // Select and activate the initial spotlights
        for (int i = 0; i < initialActiveSpotlights; i++)
        {
            ChooseANewOne();
        }

        // Start the coroutine to continuously check and maintain the number of active spotlights
        StartCoroutine(CheckAndMaintainActiveSpotlights());
    }

    IEnumerator CheckAndMaintainActiveSpotlights()
    {
        while (true)
        {
            // Check if the number of active spotlights is less than the desired number
            if (activeSpotlights.Count < initialActiveSpotlights && inactiveSpotlights.Count > 0)
            {
                //Debug.Log("Here");
                // Choose a new random spotlight to activate
                ChooseANewOne();
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void ChooseANewOne()
    {
        if (inactiveSpotlights.Count == 0)
        {
            Debug.LogWarning("No inactive spotlights available to activate.");
            return;
        }

        // Choose a new random spotlight from the inactive list
        int randomIndex = Random.Range(0, inactiveSpotlights.Count);
        GameObject newSpotlight = inactiveSpotlights[randomIndex];

        // Remove it from the inactive list and add it to the active list
        inactiveSpotlights.RemoveAt(randomIndex);
        activeSpotlights.Add(newSpotlight);

        // Turn on the new spotlight
        SpotLightMove spotlightMove = newSpotlight.GetComponent<SpotLightMove>();
        if (spotlightMove != null)
        {
            spotlightMove.spotlightManager = gameObject;
            spotlightMove.TurnMeOn();
        }
        else
        {
            Debug.LogError("No SpotLightMove component found on the spotlight.");
        }
    }

    public void FreezeSpotlight()
    {
        StartCoroutine(FreezeMoveSpotlight());
    }


    IEnumerator FreezeMoveSpotlight()
    {
        foreach (GameObject spotlight in spotlights)
        {
            spotlight.GetComponent<SpotLightMove>().inFreeze = true;
        }
        yield return new WaitForSeconds(freezLightInterval);
        foreach (GameObject spotlight in spotlights)
        {
            spotlight.GetComponent<SpotLightMove>().inFreeze = false;
        }
    }

    public void TurnOffSpotlight()
    {
        StartCoroutine(TurnOffAllSpotlight(turnoffLightInterval));
    }

    public void CentralSpotlight()
    {
        StartCoroutine(TurnOffAllSpotlight(2));
        StartCoroutine(CentralSpotlightOn());
    }


    IEnumerator TurnOffAllSpotlight(float delay)
    {
        foreach (GameObject spotlight in spotlights)
        {
            spotlight.GetComponent<SpotLightMove>().inOff = true;
            spotlight.GetComponent<Light>().intensity = 0;
        }
        yield return new WaitForSeconds(delay);
        foreach (GameObject spotlight in spotlights)
        {
            spotlight.GetComponent<SpotLightMove>().inOff = false;
        }
    }

    IEnumerator CentralSpotlightOn()
    {
        centralSpotlight.GetComponent<Light>().intensity = 5000;
        activeSpotlights.Add(centralSpotlight);
        yield return new WaitForSeconds(2);
        activeSpotlights.Remove(centralSpotlight);
        centralSpotlight.GetComponent<Light>().intensity = 0;

    }

    public void Pause()
    {
        List<string> activeSpotlightNames = new List<string>();
        foreach (GameObject spotlight in spotlights)
        {
            spotlight.GetComponent<SpotLightMove>().Pause();
        }
        foreach (GameObject spotlight in activeSpotlights)
        {
            activeSpotlightNames.Add(spotlight.name);
        }
        ES3.Save("activeSpotlights", activeSpotlightNames);
    }

    // Read the saved active spotlights and call the Resume method on them
    public void Resume()
    {
        if (ES3.KeyExists("activeSpotlights"))
        {
            List<string> activeSpotlightNames = ES3.Load<List<string>>("activeSpotlights");

            foreach (string spotlightName in activeSpotlightNames)
            {
                GameObject spotlight = spotlights.Find(s => s.name == spotlightName);
                if (spotlight != null)
                {
                    SpotLightMove spotlightMove = spotlight.GetComponent<SpotLightMove>();
                    if (spotlightMove != null)
                    {
                        spotlightMove.spotlightManager = gameObject;
                        spotlightMove.Resume();
                    }
                    else
                    {
                        Debug.LogError("No SpotLightMove component found on the spotlight.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Spotlight with name {spotlightName} not found.");
                }
            }
        }
    }

    public void TurnOffSpotlight(GameObject spotlight)
    {
        //Debug.Log("Here");
        if (activeSpotlights.Contains(spotlight))
        {
            activeSpotlights.Remove(spotlight);
            inactiveSpotlights.Add(spotlight);
        }
    }
}
