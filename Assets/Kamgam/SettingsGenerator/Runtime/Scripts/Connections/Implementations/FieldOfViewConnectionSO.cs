using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "FieldOfViewConnection", menuName = "SettingsGenerator/Connection/FieldOfViewConnection", order = 4)]
    public class FieldOfViewConnectionSO : FloatConnectionSO
    {
        public bool UseMain = true;
        public bool UseMarkers = true;

        protected FieldOfViewConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new FieldOfViewConnection(UseMain, UseMarkers);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
