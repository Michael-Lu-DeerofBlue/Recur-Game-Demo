using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "ShadowDistanceConnection", menuName = "SettingsGenerator/Connection/ShadowDistanceConnection", order = 4)]
    public class ShadowDistanceConnectionSO : OptionConnectionSO
    {
        [Tooltip("You can provide your own max shadow distances per Quality Level. Index 0 = Quality Level 0, ... . \n" +
            "If UseQualitySettingsAsFallback is enabled then you can leave this empty.")]
        public List<float> QualityDistances;

        [Tooltip("Use the max shadow distances from the QualitySettings or RenderPipelineAssets. \n" +
            "NOTICE: If you are using the BuiltIn (legacy) renderer then this will cause a single cycle through all qualities at first use.")]
        public bool UseQualitySettingsAsFallback = true;

        protected ShadowDistanceConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new ShadowDistanceConnection(QualityDistances, UseQualitySettingsAsFallback);
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
