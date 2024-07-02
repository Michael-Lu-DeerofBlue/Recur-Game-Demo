using UnityEngine;
using UnityEngine.Audio;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "AudioMixerParameterConnection", menuName = "SettingsGenerator/Connection/AudioMixerParameterConnection", order = 4)]
    public class AudioMixerParameterConnectionSO : FloatConnectionSO
    {
        [Tooltip("The mixer that should be controlled by this connection.")]
        public AudioMixer Mixer;

        [Tooltip("The name of the exposed parameter.")]
        public string ExposedParameterName;

        protected AudioMixerParameterConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new AudioMixerParameterConnection(Mixer, ExposedParameterName);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
