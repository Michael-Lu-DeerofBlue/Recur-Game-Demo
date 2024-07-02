using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "GammaVolumeConnection", menuName = "SettingsGenerator/Connection/GammaVolumeConnection", order = 4)]
    public class GammaVolumeConnectionSO : FloatConnectionSO
    {
        protected GammaVolumeConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new GammaVolumeConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
