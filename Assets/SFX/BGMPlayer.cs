using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kamgam.SettingsGenerator;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;
    public AudioSource[] AudioSources;
    public AudioSourceType[] SourceTypes;
    public Kamgam.SettingsGenerator.SettingsProvider Provider;

    [Tooltip("How the input should be mapped to the required output of 0f..1f (X = min, Y = max).\n" +
             "Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.")]
    public Vector2 InputRange = new Vector2(0f, 100f);

    [System.NonSerialized] public AudioSourceVolumeConnection EffectConnection;
    [System.NonSerialized] public AudioSourceVolumeConnection MusicConnection;

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
    }

    public void Start()
    {
        for (int i = 0; i < AudioSources.Length; i++)
        {
            if (SourceTypes[i] == AudioSourceType.Effect)
            {
                ConnectVolume("audioEffectVolume", AudioSources[i], ref EffectConnection);
            }
            else if (SourceTypes[i] == AudioSourceType.Music)
            {
                ConnectVolume("audioMusicVolume", AudioSources[i], ref MusicConnection);
            }
        }
    }

    private void ConnectVolume(string volumeId, AudioSource audioSource, ref AudioSourceVolumeConnection connection)
    {
        var setting = SettingsInitializer.Settings.GetFloat(id: volumeId);
        var volumeAdjuster = audioSource.GetComponent<BaseVolumeAdjuster>();

        if (!setting.HasConnection())
        {
            connection = new AudioSourceVolumeConnection(InputRange, new AudioSource[] { audioSource });
            setting.SetConnection(connection);
        }
        else
        {
            connection = setting.GetConnection() as AudioSourceVolumeConnection;
            if (connection != null)
            {
                connection.AddAudioSources(new AudioSource[] { audioSource });
            }
        }

        // Update volumeAdjuster when the setting changes
        setting.OnValueChanged += (volume) =>
        {
            if (volumeAdjuster != null)
            {
                volumeAdjuster.UpdateVolume(volume);
            }
            else
            {
                // Apply default volume adjustment of 1.0f
                audioSource.volume = volume;
            }
        };
        setting.Apply();
    }
}

public enum AudioSourceType
{
    Effect,
    Music
}


    // private void FixedUpdate()
    // {
    //     var settings = SettingsInitializer.Settings;
    //     if (isEffect)
    //     {

    //     }
    //     else if (isMusic)
    //     {
    //         SettingFloat volume = settings.GetFloat(id: "audioMusicVolume");
    //         audioSource.volume = volume.GetFloatValue()/100f;
    //     }
    // }

    // public void ChangeAudioClip(AudioClip newClip)
    // {
    //     if (audioSource != null)
    //     {
    //         audioSource.clip = newClip;
    //         audioSource.Play();
    //     }
    // }
