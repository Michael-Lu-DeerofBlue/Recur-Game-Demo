using System.Collections;
using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    /// <summary>
    /// This component mutes the normal music for "Duration" seconds and plays the music through a mixer instead.<br />
    /// This is deon for testing the mixer connection.
    /// </summary>
    public class PlayMusicThroughAudioMixer : MonoBehaviour
    {
        public SettingsProvider SettingsProvider;
        public string musicSettingId;
        public float Duration = 5f;

        public AudioSource SourceManagedByMixer;

        protected SettingFloat _musicVolumeSetting;
        public SettingFloat MusicVolumeSetting
        {
            get
            {
                if (_musicVolumeSetting == null && SettingsProvider != null && SettingsProvider.HasSettings())
                {
                    _musicVolumeSetting = SettingsProvider.Settings.GetFloat(musicSettingId);
                }
                return _musicVolumeSetting;
            }
        }

        protected bool _isPlaying = false;
        protected float _musicVolume;

        public void Toggle()
        {
            _isPlaying = !_isPlaying;
            if(_isPlaying)
            {
                play();
            }
            else
            {
                stop();
            }
        }

        void play()
        {
            StartCoroutine(playAndStopAfterNSeconds(Duration));
        }

        IEnumerator playAndStopAfterNSeconds(float seconds)
        {
            // silence the normal music
            _musicVolume = MusicVolumeSetting.GetValue();
            MusicVolumeSetting.SetValue(0f);

            // Play music from mixer.
            SourceManagedByMixer.Play();
            yield return new WaitForSeconds(seconds);

            stop();
        }

        void stop()
        {
            StopAllCoroutines();
            SourceManagedByMixer.Stop();

            // Restore normal music volume
            MusicVolumeSetting.SetValue(_musicVolume);

            _isPlaying = false;
        }
    }
}
