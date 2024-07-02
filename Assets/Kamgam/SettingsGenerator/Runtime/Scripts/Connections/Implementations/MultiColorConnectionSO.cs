using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MultiColorConnection", menuName = "SettingsGenerator/Connection/MultiColorConnection", order = 4)]
    public class MultiColorConnectionSO : ColorOptionConnectionSO
    {
        protected MultiColorConnection _connection;

        public override IConnectionWithOptions<Color> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MultiColorConnection(0);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
