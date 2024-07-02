using UnityEngine;
using UnityEngine.Audio;

namespace Kamgam.SettingsGenerator
{
    public class AudioMixerParameterConnection : Connection<float>
    {
        /// <summary>
        /// The mixer that should be controlled by this connection.
        /// </summary>
        public AudioMixer Mixer;

        /// <summary>
        /// The name of the exposed parameter.
        /// </summary>
        public string ExposedParameterName;

        public AudioMixerParameterConnection(AudioMixer mixer, string exposedParameterName)
        {
            Mixer = mixer;
            ExposedParameterName = exposedParameterName;
        }

        public override float Get()
        {
            float value;
            if (Mixer.GetFloat(ExposedParameterName, out value))
            {
                return value;
            }
            else
            {
                return 0f;
            }
        }

        public override void Set(float value)
        {
            // In the Editor the Snapshot of the AudioMixer is actually applied
            // in Awake() and thus (based on the Awake exectution order) it may
            // override the changes made here since settings are also loaded and
            // applied in Awake.
            //
            // The current solution is to delay by one frame if the current frame is 0.
            // It's not ideal and the proper solution would be to set in Awake() and then
            // also set in Start(). However, that is not controlled by this object.
            //
            // See: https://fogbugz.unity3d.com/default.asp?1197165_nik4gg1io942ae13
            // and: https://forum.unity.com/threads/audiomixer-setfloat-doesnt-work-on-awake.323880/
            
            // Always set immediately
            Mixer.SetFloat(ExposedParameterName, value);

            // If it is the very first frame then also schedule a set one frame later.
            // We use async since that works independently of any object lifecycle.
            if (Time.frameCount < 1)
            {
                if (!_scheduledDelayedSet)
                {
                    _scheduledDelayedSet = true;
                    setOneFrameLater(value);
                }
            }
        }

        protected bool _scheduledDelayedSet = false;

        protected async void setOneFrameLater(float value)
        {
            // Wait 10 MS (since the async context is updated only once every frame this gives us a 1 frame delay if fps is below 100).
            await System.Threading.Tasks.Task.Delay(10);

            // Set again.
            Mixer.SetFloat(ExposedParameterName, value);

            _scheduledDelayedSet = false;
        }
    }
}
