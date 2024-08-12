using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelMusic : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;

    void Start()
    {
        //PlayAudio();
    }

    void PlayAudio()
    {
        // Stop any currently playing audio
        StopAllAudio();

        if (audioSource1 != null && audioSource2 != null)
        {
            if (audioSource1.clip != null && audioSource2.clip != null)
            {
                Debug.Log("Playing both audio sources.");
                PlayMusic(audioSource1);
                PlayMusic(audioSource2);
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

    void PlayMusic(AudioSource _audioSource)
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    void StopMusic(AudioSource _audioSource)
    {
        _audioSource.Stop();
    }

    void StopAllAudio()
    {
        if (audioSource1 != null && audioSource1.isPlaying)
        {
            StopMusic(audioSource1);
        }

        if (audioSource2 != null && audioSource2.isPlaying)
        {
            StopMusic(audioSource2);
        }
    }
}
