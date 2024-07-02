#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public partial class ShadowConnection
    {
        // Volume based setting. This means it will generate a volume game object
        // with a high priority to override the default settings made by other volumes.

        // This could also be implemented as a by-camera setting using the FrameSettingsField.ShadowMaps
        // of the camera HD data.

        protected SettingsVolumeShadowControlHDRP settingsVolumeShadowControlHDRP;

        public override bool Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return true;

            if (settingsVolumeShadowControlHDRP == null)
                settingsVolumeShadowControlHDRP = SettingsVolume.Instance.GetOrCreateControl<SettingsVolumeShadowControlHDRP>();

            return settingsVolumeShadowControlHDRP.ShadowsEnabled;
        }

        public override void Set(bool enable)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            if (settingsVolumeShadowControlHDRP == null)
                settingsVolumeShadowControlHDRP = SettingsVolume.Instance.GetOrCreateControl<SettingsVolumeShadowControlHDRP>();

            // Shadow settings on override volume
            settingsVolumeShadowControlHDRP.ShadowsEnabled = enable;

            NotifyListenersIfChanged(enable);
        }
    }
}

#endif
