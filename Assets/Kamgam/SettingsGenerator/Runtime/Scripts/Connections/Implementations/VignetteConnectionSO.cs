using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "VignetteConnection", menuName = "SettingsGenerator/Connection/VignetteConnection", order = 4)]
    public class VignetteConnectionSO : BoolConnectionSO
    {
        protected VignetteConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new VignetteConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
