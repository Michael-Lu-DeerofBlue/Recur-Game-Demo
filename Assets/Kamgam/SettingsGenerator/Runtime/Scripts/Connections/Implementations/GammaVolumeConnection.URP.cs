#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Kamgam.SettingsGenerator
{
    public partial class GammaVolumeConnection : Connection<float>
    {
        // Volume based setting. This means it will generate a volume game object
        // with a high priority to override the default settings made by other volumes.

        protected LiftGammaGain _effect;
        public Vector4 _defaultValue = new Vector4(1f, 1f, 1f, 0f);

        public GammaVolumeConnection()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            // Add the component to the settings volume to override all other.
            _effect = SettingsVolume.Instance.GetOrAddComponent<LiftGammaGain>();
            _effect.Override(_effect, 1f);
            _effect.active = false;

            UpdateDefaultValue();
        }

        /// <summary>
        /// Tries to find the default gamma values based on all the existing volumes.<br />
        /// This value is used as base for the gamma correction.
        /// </summary>
        public void UpdateDefaultValue()
        {
            // Warn if there is no default effect
            var defaultEffect = SettingsVolume.Instance.FindDefaultVolumeComponent<LiftGammaGain>();
            if (defaultEffect != null)
            {
                _defaultValue = defaultEffect.gamma.value;
            }
        }

        public override float Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (_effect == null || _effect.active == false)
                return 0f;

            if (_effect.gamma.overrideState)
            {
                return _effect.gamma.value.w;
            }

            return 0f;
        }

        public override void Set(float gamma)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (_effect == null)
                return;

            _effect.active = true;

            var newValue = _defaultValue;
            newValue.w = gamma;
            _effect.gamma.Override(newValue);

            NotifyListenersIfChanged(gamma);
        }
    }
}

#endif