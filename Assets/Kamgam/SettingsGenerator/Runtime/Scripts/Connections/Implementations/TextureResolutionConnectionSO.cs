using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "TextureResolutionConnection", menuName = "SettingsGenerator/Connection/TextureResolutionConnection", order = 4)]
    public class TextureResolutionConnectionSO : OptionConnectionSO
    {
        protected TextureResolutionConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new TextureResolutionConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
