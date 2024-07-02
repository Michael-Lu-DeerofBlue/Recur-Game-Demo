using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class AudioVolumeConnection : Connection<float>
    {
        /// <summary>
        /// How the input should be mapped to 0f..1f.<br />
        /// Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.
        /// </summary>
        public Vector2 InputRange = new Vector2(0f, 100f);

        public AudioVolumeConnection(Vector2 inputRange)
        {
            InputRange = inputRange;
        }

        public override float Get()
        {
            return MathUtils.MapWithAnchor(AudioListener.volume, 0f, 0f, 1f, InputRange.x, InputRange.x, InputRange.y, clamp: false);
        }

        public override void Set(float volume)
        {
            AudioListener.volume = MathUtils.MapWithAnchor(volume, InputRange.x, InputRange.x, InputRange.y, 0f, 0f, 1f, clamp: false);
        }
    }
}
