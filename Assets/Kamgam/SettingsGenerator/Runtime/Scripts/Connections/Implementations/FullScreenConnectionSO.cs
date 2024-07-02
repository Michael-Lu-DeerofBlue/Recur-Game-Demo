using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "FullScreenConnection", menuName = "SettingsGenerator/Connection/FullScreenConnection", order = 4)]
    public class FullScreenConnectionSO : BoolConnectionSO
    {
        protected FullScreenConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new FullScreenConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
