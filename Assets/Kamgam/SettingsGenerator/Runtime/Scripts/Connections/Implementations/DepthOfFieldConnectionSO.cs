using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "DepthOfFieldConnection", menuName = "SettingsGenerator/Connection/DepthOfFieldConnection", order = 4)]
    public class DepthOfFieldConnectionSO : BoolConnectionSO
    {
        protected DepthOfFieldConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new DepthOfFieldConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
