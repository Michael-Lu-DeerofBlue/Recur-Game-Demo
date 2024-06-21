using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public TrackCollectible trackCollectible;
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        string otherName = other.tag;
        if (other.CompareTag("Player"))
        {
            Debug.Log("You got a key piece!");
            trackCollectible.AddKeyShards();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, Time.time * 100f, 90f);
    }
}