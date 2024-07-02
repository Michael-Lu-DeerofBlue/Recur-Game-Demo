using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "AmbientOcclusionConnection", menuName = "SettingsGenerator/Connection/AmbientOcclusionConnection", order = 4)]
    public class AmbientOcclusionConnectionSO : BoolConnectionSO
    {
        protected AmbientOcclusionConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new AmbientOcclusionConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
