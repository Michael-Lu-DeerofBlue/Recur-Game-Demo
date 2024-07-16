using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private Dictionary<string, AudioClip> audioClipDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeAudioClipDictionary();
    }

    private void InitializeAudioClipDictionary()
    {
        audioClipDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips)
        {
            if (clip != null)
            {
                audioClipDictionary[clip.name] = clip;
            }
        }
    }

    public void PlaySound(string clipName)
    {
        if (audioClipDictionary.TryGetValue(clipName, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Audio clip not found: " + clipName);
        }
    }
}


