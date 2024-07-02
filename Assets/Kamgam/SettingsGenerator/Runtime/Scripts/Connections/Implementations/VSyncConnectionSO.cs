using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "VSyncConnection", menuName = "SettingsGenerator/Connection/VSyncConnection", order = 1)]
    public class VSyncConnectionSO : BoolConnectionSO
    {
        protected VSyncConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new VSyncConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
