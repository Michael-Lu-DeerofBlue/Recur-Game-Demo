using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CreateAssetMenu(fileName = "ShadowResolutionConnection", menuName = "SettingsGenerator/Connection/ShadowResolutionConnection", order = 4)]
    public class ShadowResolutionConnectionSO : OptionConnectionSO
    {
#if KAMGAM_RENDER_PIPELINE_HDRP
        [Tooltip("If a light has a custom shadow map resolution set" +
            " instead of a level then this defines whether or not" +
            " to override the custom resolution.\n\n" +
            "NOTICE: If you uncheck this and you have a light with a" +
            " custom shadow map resolution set then the 'ShadowResolution'" +
            " setting will have no effect on that light.")]
        public bool OverrideCustomResolution = true;
#endif

        protected ShadowResolutionConnection _connection;

        public override IConnectionWithOptions<string> GetConnection()
        {
            if(_connection == null)
                Create();

            return _connection;
        }

        public void Create()
        {
            _connection = new ShadowResolutionConnection();

#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP
            _connection.SetOverrideCustomShadowResolution(OverrideCustomResolution);
#endif
        }

        public override void DestroyConnection()
        {
            if (_connection != null)
                _connection.Destroy();

            _connection = null;
        }
    }
}
