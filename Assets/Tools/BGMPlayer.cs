using UnityEngine;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Kamgam.SettingsGenerator;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;
    private AudioSource audioSource;
    public Kamgam.SettingsGenerator.SettingsProvider Provider;

    void Awake()
    {
        // Check if there is already an instance of BGMPlayer
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance to this
        instance = this;

        // Make this GameObject persist across scenes
        DontDestroyOnLoad(gameObject);

        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Check if an AudioSource is attached and play the audio
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource component missing on this GameObject.");
        }
    }

    private void FixedUpdate()
    {
        var settings = SettingsInitializer.Settings; 
        SettingFloat volume = settings.GetFloat(id: "audioMusicVolume");
        audioSource.volume = volume.GetFloatValue()/100f;
    }

    public void ChangeAudioClip(AudioClip newClip)
    {
        if (audioSource != null)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}
