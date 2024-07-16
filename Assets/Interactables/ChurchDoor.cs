using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Door : Interactable
{
    public Flowchart flowchart;
    public GameObject Player;
    public GameObject levelScript;
    public bool Keyed;
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
        if (Keyed)
        {
            PlayEffectDoor();
            flowchart.ExecuteBlock("DoorRotates");
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {

        }
    }

    void PlayEffectDoor()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart for sprinting
            //flowchart.ExecuteBlock("Door");
        }
    }
}