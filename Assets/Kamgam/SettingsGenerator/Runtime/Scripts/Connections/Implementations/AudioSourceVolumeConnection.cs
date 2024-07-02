using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class AudioSourceVolumeConnection : Connection<float>
    {
        [Tooltip("How the input should be mapped to 0f..1f.\n" +
            "Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.")]
        public Vector2 InputRange = new Vector2(0f, 100f);

        public List<AudioSource> AudioSources;

        public AudioSourceVolumeConnection(Vector2 inputRange, IList<AudioSource> audioSources)
        {
            InputRange = inputRange;
            AddAudioSources(audioSources);
        }

        public void AddAudioSources(IList<AudioSource> audioSources)
        {
            if (AudioSources == null)
            {
                AudioSources = new List<AudioSource>();
            }

            AudioSources.AddRange(audioSources);
        }

        public override float Get()
        {
            if (AudioSources.IsNullOrEmpty() || AudioSources[0] == null || AudioSources[0].gameObject == null)
                return MathUtils.MapWithAnchor(0.5f, 0f, 0f, 1f, InputRange.x, InputRange.x, InputRange.y);

            return MathUtils.MapWithAnchor(AudioSources[0].volume, 0f, 0f, 1f, InputRange.x, InputRange.x, InputRange.y, clamp: false);
        }

        public override void Set(float value)
        {
            if (AudioSources.IsNullOrEmpty())
                return;

            foreach (var source in AudioSources)
            {
                if (source == null || source.gameObject == null)
                    continue;

                source.volume = MathUtils.MapWithAnchor(value, InputRange.x, InputRange.x, InputRange.y, 0f, 0f, 1f, clamp: false);
            }
        }
    }
}
