// The custom defines KAMGAM_RENDER_PIPELINE_CORE and KAMGAM_RENDER_PIPELINE_HDRP
// Are coming from the runtime assembly definition (see Version Defines).

#if KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public class SettingsVolumeAmbientLightControlHDRP : ISettingsVolumeControl
    {
        protected IndirectLightingController indirectLightController;

        public void Initialize(SettingsVolume settingsVolume)
        {
            if (settingsVolume == null)
                return;

            indirectLightController = settingsVolume.GetOrAddComponent<IndirectLightingController>();
            
            // init with default values
            var defaultController = settingsVolume.FindDefaultVolumeComponent<IndirectLightingController>(useStackAsFallback: true);
            if (defaultController != null)
            {
                Intensity = indirectLightController.indirectDiffuseLightingMultiplier.value;
            }
            else
            {
                Intensity = 1f;
            }

        }

        public float Intensity
        {
            get
            {
                indirectLightController.indirectDiffuseLightingMultiplier.overrideState = true;
                return indirectLightController.indirectDiffuseLightingMultiplier.value;
            }

            set
            {
                indirectLightController.indirectDiffuseLightingMultiplier.overrideState = true;

                if (indirectLightController.indirectDiffuseLightingMultiplier.value != value)
                    indirectLightController.indirectDiffuseLightingMultiplier.value = value;
            }
        }
    }
}
#endif
