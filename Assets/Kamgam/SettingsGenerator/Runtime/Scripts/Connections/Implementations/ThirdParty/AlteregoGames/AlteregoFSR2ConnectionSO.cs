using UnityEngine;
using UnityEngine.Audio;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "AlteregoFSR2Connection", menuName = "SettingsGenerator/Connection/AlteregoFSR2Connection", order = 4)]
    public class AlteregoFSR2ConnectionSO : OptionConnectionSO
    {
        protected AlteregoFSR2Connection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new AlteregoFSR2Connection(); 
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
