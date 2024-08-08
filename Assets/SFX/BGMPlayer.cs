using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kamgam.SettingsGenerator;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;
    public AudioSource[] AudioSources;
    public AudioSourceType[] SourceTypes;
    public AudioSource[] allAudioSources;
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

        allAudioSources = FindObjectsOfType<AudioSource>();
    }

    private void ConnectVolume(string volumeId, AudioSource audioSource, ref AudioSourceVolumeConnection connection)
    {
        var setting = SettingsInitializer.Settings.GetFloat(id: volumeId);
        var baseVolumeAdjuster = audioSource.GetComponent<BaseVolumeAdjuster>();

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

        // Apply volume immediately
        float currentVolume = setting.GetValue();
        currentVolume = ApplyVolumeAdjusters(currentVolume, baseVolumeAdjuster);
        audioSource.volume = currentVolume;

        // Update volumeAdjuster when the setting changes
        setting.OnValueChanged += (volume) =>
        {
            volume = ApplyVolumeAdjusters(volume, baseVolumeAdjuster);
            audioSource.volume = volume;
        };
        setting.Apply();
    }

    private float ApplyVolumeAdjusters(float volume, BaseVolumeAdjuster baseVolumeAdjuster)
    {
        if (baseVolumeAdjuster != null)
        {
            volume *= baseVolumeAdjuster.baseVolume;
        }
        return volume;
    }

    public void PlayMusic(AudioSource _audioSource)
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic(AudioSource _audioSource)
    {
        _audioSource.Stop();
    }

    public void StopAllMusic()
    {
        for (int i = 0; i < allAudioSources.Length; i++)
        {
            if (allAudioSources[i] != null && allAudioSources[i].isPlaying)
            {
                StopMusic(allAudioSources[i]);
            }
        }
    }
}

public enum AudioSourceType
{
    Effect,
    Music
}