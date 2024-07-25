using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfadeAudioSource : MonoBehaviour
{
    private BaseVolumeAdjuster volumeAdjuster;

    void Awake()
    {
        volumeAdjuster = GetComponent<BaseVolumeAdjuster>();
    }

    public void Fade(AudioClip clip, float volume)
    {
        StartCoroutine(FadeIt(clip, volume));
    }

    IEnumerator FadeIt(AudioClip clip, float volume)
    {
        // Apply the base volume adjuster multiplier to the volume
        float adjustedVolume = volumeAdjuster != null ? volume * volumeAdjuster.baseVolume : volume;

        // Add new AudioSource and set it to all parameters of the original AudioSource
        AudioSource fadeOutSource = gameObject.AddComponent<AudioSource>();
        AudioSource originalSource = GetComponent<AudioSource>();

        fadeOutSource.clip = originalSource.clip;
        fadeOutSource.time = originalSource.time;
        fadeOutSource.volume = originalSource.volume;
        fadeOutSource.outputAudioMixerGroup = originalSource.outputAudioMixerGroup;

        // Make it start playing
        fadeOutSource.Play();

        // Set original AudioSource volume and clip
        originalSource.volume = 0f;
        originalSource.clip = clip;
        float t = 0;
        float v = fadeOutSource.volume;
        originalSource.Play();

        // Begin fading in original AudioSource with new clip as we fade out new AudioSource with old clip
        while (t < 0.98f)
        {
            t = Mathf.Lerp(t, 1f, Time.deltaTime * 0.2f);
            fadeOutSource.volume = Mathf.Lerp(v, 0f, t);
            originalSource.volume = Mathf.Lerp(0f, adjustedVolume, t);
            yield return null;
        }
        originalSource.volume = adjustedVolume;

        // Destroy the fading AudioSource
        Destroy(fadeOutSource);
        yield break;
    }
}
