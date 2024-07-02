using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    public class PlayAudioAtInterval : MonoBehaviour
    {
        public float Interval = 2f;
        public AudioSource Source;

        protected float _timer;

        void Update()
        {
            // Play Sound Effect every 2 seconds
            _timer += Time.deltaTime;
            if (_timer > Interval)
            {
                _timer = 0f;
                Source.Play();
            }
        }
    }
}
