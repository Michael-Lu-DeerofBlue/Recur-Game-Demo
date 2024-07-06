using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TeleportPlayerScript : MonoBehaviour
{
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
            target.SendMessage(message);
        }
    }
}
