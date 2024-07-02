using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "AmbientLightConnection", menuName = "SettingsGenerator/Connection/AmbientLightConnection", order = 4)]
    public class AmbientLightConnectionSO : FloatConnectionSO
    {
        [Tooltip("The allowed min ambient color intensity.\n" +
            "Only used if ambient light is set to color.\n" +
            "Useful to avoid losing the color info if ambient is set to black.\n" +
            "NOTICE: This is only used in Built-In and URP, not HDRP.")]
        public float MinColorIntensity = 0.01f;

        [Tooltip("Max color intensity. 2f by default. You may want to change it depending on your ambient color (HD or SD).\n" +
            "Only used if ambient light is set to color.\n" +
            "NOTICE: This is only used in Built-In and URP, not HDRP.")]
        public float MaxColorIntensity = 2f;

        protected AmbientLightConnection _connection;

        public override IConnection<float> GetConnection()
        {
            if (_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new AmbientLightConnection();
            _connection.MinColorIntensity = MinColorIntensity;
            _connection.MaxColorIntensity = MaxColorIntensity;
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}