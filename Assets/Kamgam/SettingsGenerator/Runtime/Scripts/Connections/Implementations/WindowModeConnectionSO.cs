using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "WindowModeConnection", menuName = "SettingsGenerator/Connection/WindowModeConnection", order = 4)]
    public class WindowModeConnectionSO : OptionConnectionSO
    {
        protected WindowModeConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new WindowModeConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
