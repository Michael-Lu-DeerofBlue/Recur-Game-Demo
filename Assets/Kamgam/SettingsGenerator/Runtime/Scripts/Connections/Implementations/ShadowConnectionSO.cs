using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "ShadowConnection", menuName = "SettingsGenerator/Connection/ShadowConnection", order = 4)]
    public class ShadowConnectionSO : BoolConnectionSO
    {
        protected ShadowConnection _connection;

        public override IConnection<bool> GetConnection()
        {
            if (_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new ShadowConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
