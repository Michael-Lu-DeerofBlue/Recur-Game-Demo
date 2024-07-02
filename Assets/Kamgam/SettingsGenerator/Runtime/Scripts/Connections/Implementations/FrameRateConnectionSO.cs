using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "FrameRateConnection", menuName = "SettingsGenerator/Connection/FrameRateConnection", order = 4)]
    public class FrameRateConnectionSO : OptionConnectionSO
    {
        protected FrameRateConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new FrameRateConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
