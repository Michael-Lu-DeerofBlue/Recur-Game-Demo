#if KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public class SettingsVolumeShadowControlHDRP : ISettingsVolumeControl
    {
        protected HDShadowSettings _shadowSettings;
        protected ContactShadows _contactShadows;

        protected bool _shadowsEnabled;
        protected float _shadowDistanceIfActive;
        protected bool _usesContactShadows;

        public void Initialize(SettingsVolume settingsVolume)
        {
            if (settingsVolume == null)
                return;

            // Setup to disable shadows via max distance = 0.
            _shadowSettings = settingsVolume.GetOrAddComponent<HDShadowSettings>();
            _shadowSettings.Override(_shadowSettings, 1f);
            // init with default values
            var defaultHDShadowSettings = settingsVolume.FindDefaultVolumeComponent<HDShadowSettings>(useStackAsFallback: true);
            if (defaultHDShadowSettings != null)
            {
                _shadowSettings.maxShadowDistance.value = defaultHDShadowSettings.maxShadowDistance.value;
                _shadowSettings.maxShadowDistance.overrideState = true;

                _shadowsEnabled = _shadowSettings.maxShadowDistance.value > 0f;
                _shadowDistanceIfActive = _shadowSettings.maxShadowDistance.value;
            }
            else
            {
                _shadowsEnabled = true;
                _shadowDistanceIfActive = 100f;

                _shadowSettings.maxShadowDistance.value = _shadowDistanceIfActive;
                _shadowSettings.maxShadowDistance.overrideState = false;
            }

            // Setup to disable contact shadows
            _contactShadows = settingsVolume.GetOrAddComponent<ContactShadows>();
            // init with default values
            var defaultContactShadows = settingsVolume.FindDefaultVolumeComponent<ContactShadows>(useStackAsFallback: true);
            if (defaultContactShadows != null)
            {
                _usesContactShadows = true;
                _contactShadows.enable.value = defaultContactShadows.enable.value;
                _contactShadows.enable.overrideState = true;
            }
            else
            {
                _usesContactShadows = false;
                _contactShadows.enable.value = false;
                _contactShadows.enable.overrideState = false;
            }
        }

        public bool ShadowsEnabled
        {
            get
            {
                return _shadowSettings.maxShadowDistance.value > 0.01f;
            }

            set
            {
                _shadowsEnabled = value;
                apply();
            }
        }

        public float ShadowDistance
        {
            get
            {
                if (_shadowsEnabled)
                {
                    return _shadowSettings.maxShadowDistance.value;
                }
                else
                {
                    return _shadowDistanceIfActive;
                }
            }

            set
            {
                _shadowDistanceIfActive = value;
                apply();
            }
        }

        public float? DefaultShadowDistance
        {
            get
            {
                // If this is called during EDIT Mode then the Instance will be null.
                if (SettingsVolume.Instance == null)
                    return null;

                var defaultComp = SettingsVolume.Instance.FindDefaultVolumeComponent<HDShadowSettings>();
                if (defaultComp != null)
                {
                    return defaultComp.maxShadowDistance.value;
                }

                return null;
            }
        }

        protected void apply()
        {
            // Contact shadows on/off
            if (_usesContactShadows)
            {
                _contactShadows.enable.overrideState = !_shadowsEnabled;
            }

            // Shadow distance (always on but distance depends on enabled state).
            _shadowSettings.maxShadowDistance.overrideState = true;
            if(_shadowsEnabled)
            {
                _shadowSettings.maxShadowDistance.value = _shadowDistanceIfActive;
            }
            else
            {
                _shadowSettings.maxShadowDistance.value = 0f;
            }
        }
    }
}
#endif
