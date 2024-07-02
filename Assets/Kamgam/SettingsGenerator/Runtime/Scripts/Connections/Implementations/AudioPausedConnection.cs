using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class AudioPausedConnection : Connection<bool>
    {
        /// <summary>
        /// If this should be used as an audo enable/disable connection then
        /// invert the meaning of the bool (paused = true > false).
        /// </summary>
        public bool Invert;

        public AudioPausedConnection(bool invert = true)
        {
            Invert = invert;
        }

        public override bool Get()
        {
            return Invert ? !AudioListener.pause : AudioListener.pause;
        }

        public override void Set(bool pause)
        {
            AudioListener.pause = Invert ? !pause : pause;
        }
    }
}
