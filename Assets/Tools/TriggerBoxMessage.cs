using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TriggerBoxMessage : MonoBehaviour
{
    public GameObject trigger;
    public GameObject target;
    public string message;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered trigger: " + trigger);
            target.SendMessage(message);
        }
    }
}
