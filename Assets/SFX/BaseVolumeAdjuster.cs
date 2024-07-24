using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVolumeAdjuster : MonoBehaviour
{
    public float baseVolume = 1.0f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on this GameObject.");
        }
    }

    public void UpdateVolume(float settingVolume)
    {
        if (audioSource != null)
        {
            audioSource.volume = baseVolume * settingVolume;
        }
    }
}
