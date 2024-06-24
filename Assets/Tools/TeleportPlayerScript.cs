using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TeleportPlayerScript : MonoBehaviour
{
    public Flowchart flowchart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flowchart.ExecuteBlock("WhiteScreen");
        }
        
    }
}
