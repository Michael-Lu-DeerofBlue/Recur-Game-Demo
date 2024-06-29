using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TeleportPlayerScript : MonoBehaviour
{
    public Flowchart flowchart;
    public string BlockName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
        
    }
}
