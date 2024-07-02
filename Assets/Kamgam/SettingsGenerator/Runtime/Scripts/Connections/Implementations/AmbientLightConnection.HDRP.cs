#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public partial class AmbientLightConnection : Connection<float>
    {
        protected Color _defaultAmbientColor;
        protected float _defaultColorMaxIntensity;
        protected SettingsVolumeAmbientLightControlHDRP _settingsVolumeFieldAmbientLightHDRP;

        protected bool _warnedAboutMissingEnvironmentInEditor = false;

        public AmbientLightConnection()
        {
            _defaultAmbientColor = RenderSettings.ambientLight;
            _defaultColorMaxIntensity = Mathf.Max(_defaultAmbientColor.r, _defaultAmbientColor.g, _defaultAmbientColor.b);
        }

        /// <summary>
        /// It's close to what most people expect if they set a "Brightness" value in a game.<br />
        /// Actually it is the ambient light intensity (0% to 100%). Think of it as a percentage with 50 as the default.
        /// </summary>
        /// <returns>The "intensity" value between 0% and 100%.</returns>
        public override float Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return 50f;

            if (_settingsVolumeFieldAmbientLightHDRP == null)
                _settingsVolumeFieldAmbientLightHDRP = SettingsVolume.Instance.GetOrCreateControl<SettingsVolumeAmbientLightControlHDRP>();

            // Use intensity values from 0 to 30 (default 1).
            var mappedValue = MathUtils.MapWithAnchor(_settingsVolumeFieldAmbientLightHDRP.Intensity, 0f, 1f, 30f, 0f, 50f, 100f);
            return mappedValue;
        }

        /// <summary>
        /// Sets the "intensity" value between 0f and 100f (think of it as a percentage with 50 as the default).
        /// </summary>
        /// <param name="intensity"></param>
        public override void Set(float intensity)
        {
            warnedAboutMissingEnvironmentInEditor();

            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            if (_settingsVolumeFieldAmbientLightHDRP == null)
                _settingsVolumeFieldAmbientLightHDRP = SettingsVolume.Instance.GetOrCreateControl<SettingsVolumeAmbientLightControlHDRP>();

            // Use intensity values from 0 to 30 (default 1).
            var mappedValue = MathUtils.MapWithAnchor(intensity, 0f, 50f, 100f, 0f, 1f, 30f);
            _settingsVolumeFieldAmbientLightHDRP.Intensity = mappedValue;

            NotifyListenersIfChanged(intensity);
        }

        /// <summary>
        /// Warns the user if there is no environment set.<br />
        /// Is a NoOp outside of the Editor.
        /// </summary>
        protected void warnedAboutMissingEnvironmentInEditor()
        {
#if UNITY_EDITOR
            if (_warnedAboutMissingEnvironmentInEditor)
                return;

            _warnedAboutMissingEnvironmentInEditor = true;

            var volumes = FindComponentUtils.FindComponentsInAllLoadedScenes<Volume>(includeInactive: true);
            bool foundActive = false;
            bool found = false;
            VisualEnvironment env;
            foreach (var volume in volumes)
            {
                volume.profile.TryGet(out env);
                found |= env != null;
                foundActive |= env != null && env.active && volume.gameObject.activeInHierarchy;
            }

            if (!found && !foundActive)
            {
                Logger.Log("AmbientLightConnection: There is no Volume with a VisualEnvironment. Setting the ambient color will have no effect. " +
                    "Please add a Volume with a VisualEnvironment to your scene." +
                    "\nFind out more under: https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@15.0/manual/Override-Visual-Environment.html \n ");
            }
            else if(!foundActive)
            {
                Logger.Log("AmbientLightConnection: There is no ACTIVE Volume with a VisualEnvironment. Setting the ambient color will have no effect. " +
                    "Please ACTIVATE the VisualEnvironment on your volume. \n ");
            }
#endif
        }
    }
}
#endif