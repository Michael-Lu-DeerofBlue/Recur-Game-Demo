using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "QualityConnection", menuName = "SettingsGenerator/Connection/QualityConnection", order = 4)]
    public class QualityConnectionSO : OptionConnectionSO
    {
        [System.Obsolete("BUGFIX: The settings are now handed over automatically via the IConnectionWithSettingsAccess.SetSettings(Settings settings) method. This is no longer used and has no effect.")]
        [HideInInspector, System.NonSerialized]
        public SettingsProvider SettingsProvider;

        protected QualityConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new QualityConnection();
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
