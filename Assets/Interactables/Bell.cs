using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Bell : Interactable
{
    public Flowchart flowchart;
    
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
        Debug.Log("Interacted with" + gameObject.name);
        PlayEffectBell();
    }

    void PlayEffectBell()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart for sprinting
            flowchart.ExecuteBlock("Bell");
        }
    }

}
