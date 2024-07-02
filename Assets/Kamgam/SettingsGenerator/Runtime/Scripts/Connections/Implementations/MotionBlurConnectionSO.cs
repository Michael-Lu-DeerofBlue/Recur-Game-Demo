using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "MotionBlurConnection", menuName = "SettingsGenerator/Connection/MotionBlurConnection", order = 4)]
    public class MotionBlurConnectionSO : BoolConnectionSO
    {
        protected MotionBlurConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new MotionBlurConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
