using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelMusic : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;

    void Awake()
    {
        // Find and reference the two AudioSources on the GameObject
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            audioSource1 = audioSources[0];
            audioSource2 = audioSources[1];
        }
        else
        {
            Debug.LogError("DualAudioPlayer requires two AudioSource components on the same GameObject.");
        }
    }

    void Start()
    {
        PlayAudio();
        Debug.Log("Exited funciton.");
        Test();
    }

    void Test()
    {
        Debug.Log("Now testing regular play.");
        audioSource1.Play();
    }

    void PlayAudio()
    {
        if (audioSource1 != null && audioSource2 != null)
        {
            if (audioSource1.clip != null && audioSource2.clip != null)
            {
                Debug.Log("Playing both audio sources.");
                audioSource1.PlayOneShot(audioSource1.clip);
                audioSource2.PlayOneShot(audioSource2.clip);
            }
            else
            {
                Debug.LogError("One or both AudioSources are missing an AudioClip.");
            }
        }
        else
        {
            Debug.LogError("AudioSource references are missing.");
        }
    }
}
