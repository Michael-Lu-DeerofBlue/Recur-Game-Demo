using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "AntiAliasingConnection", menuName = "SettingsGenerator/Connection/AntiAliasingConnection", order = 4)]
    public class AntiAliasingConnectionSO : OptionConnectionSO
    {
        protected AntiAliasingConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new AntiAliasingConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
