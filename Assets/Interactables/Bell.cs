using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Bell : Interactable
{
    public Flowchart flowchart;
    public GameObject spotlightManager;
    public int spotLightCode;
    public float cdTime;
    public bool used;
    public GameObject levelScript;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Interact()
    {
        //Debug.Log("Interacted with" + gameObject.name);
        PlayEffectBell();
        levelScript.GetComponent<Level2>().ResetAllEnemyTarget(gameObject.transform);
    }

    void PlayEffectBell()
    {
        // Check if the flowchart is not already executing
        if (!used)
        {
            if (!flowchart.HasExecutingBlocks())
            {
                // Start the Fungus flowchart for sprinting
                flowchart.ExecuteBlock("Bell");
            }
            if (spotLightCode == 1)
            {
                used = true;
                spotlightManager.GetComponent<SpotlightManager>().FreezeSpotlight();
                StartCoroutine(SkillCD());
            }
            else if (spotLightCode == 2)
            {
                used = true;
                spotlightManager.GetComponent<SpotlightManager>().TurnOffSpotlight();
                StartCoroutine(SkillCD());
            }
            else if (spotLightCode == 3)
            {
                used = true;
                spotlightManager.GetComponent<SpotlightManager>().CentralSpotlight();
            }
        }
        

    }

    IEnumerator SkillCD()
    {
        yield return new WaitForSeconds(cdTime);
        used = false;
    }
}