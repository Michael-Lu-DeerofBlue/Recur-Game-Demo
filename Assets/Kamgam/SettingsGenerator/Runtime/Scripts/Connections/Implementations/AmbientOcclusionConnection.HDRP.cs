#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using UnityEngine.Rendering.HighDefinition;

#if UNITY_2022_2_OR_NEWER
// Actually it's not Unity 2022.2+ but HDRP 14+
using AmbientOcclusion = UnityEngine.Rendering.HighDefinition.ScreenSpaceAmbientOcclusion;
#endif

namespace Kamgam.SettingsGenerator
{
    public partial class AmbientOcclusionConnection : Connection<bool>
    {
        // Volume based setting. This means it will generate a volume game object
        // with a high priority to override the default settings made by other volumes.

        protected AmbientOcclusion _ambientOcclusion;

        public AmbientOcclusionConnection()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            // NOTICE:
            // Currently there is no "disable" override for VolumeComponents.
            // We achieve the same effect by teaking the values so that the
            // component no longer has any visible effect. Caveat: it will still
            // cost performance as technically it is still active.
            // TODO:
            // Implement a "per volume" mode where we actively disable the
            // VolumeComponent on the currently active volume. Editor revert
            // will be needed since it's an asset.

            // Add the component to the settings volume to override all other.
            _ambientOcclusion = SettingsVolume.Instance.GetOrAddComponent<AmbientOcclusion>();
            _ambientOcclusion.Override(_ambientOcclusion, 1f);
            _ambientOcclusion.active = false;

            // We use this only to disable the effect. We assume
            // the user has it enabled on the volumes by default.
            _ambientOcclusion.quality.overrideState = true;
            _ambientOcclusion.quality.value = 0;
            _ambientOcclusion.intensity.overrideState = true;
            _ambientOcclusion.intensity.value = 0f;


            // Warn if there is no default effect
#if UNITY_EDITOR
            var defaultBloom = SettingsVolume.Instance.FindDefaultVolumeComponent<MotionBlur>();
            if (defaultBloom == null)
            {
                var name = GetType().Name;
                var effectName = "AmbientOcclusion";
                Logger.LogWarning(
                    name + ": There was no '" + effectName + "' PostProcessing Component found on any volume. " +
                    "Please add a PostPro Volume with a profile containing '" + effectName + "'.\n\n" +
                    "Find out more here: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@13.1/manual/Volumes.html"
                    );
            }
#endif
        }

        public override bool Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (_ambientOcclusion == null)
                return true;

            // We use this only to disable the effect.
            // That's why it's inverted here. This means is the "disable override" enabled?
            return !_ambientOcclusion.active;
        }

        public override void Set(bool enable)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (_ambientOcclusion == null)
                return;

            // We use this only to disable the effect.
            // That's why it's inverted here. This means enabling the "disable override".
            _ambientOcclusion.active = !enable;

            NotifyListenersIfChanged(enable);
        }
    }
}

#endif