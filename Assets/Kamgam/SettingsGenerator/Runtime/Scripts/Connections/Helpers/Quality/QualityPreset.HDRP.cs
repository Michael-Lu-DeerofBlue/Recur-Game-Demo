#if KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public partial class QualityPreset
    {
        protected void applyToCurrentLevelHDRP()
        {
            var sourceAsset = (HDRenderPipelineAsset)renderPipeline;
            var targetAsset = (HDRenderPipelineAsset)QualitySettings.renderPipeline;

            if (sourceAsset != null && targetAsset != null)
            {
                // Commented values are read only (those are not implemented yet)

                //targetAsset.defaultParticleMaterial
                //targetAsset.defaultSpeedTree8Shader
                //targetAsset.terrainDetailGrassBillboardShader
                //targetAsset.terrainDetailGrassShader
                //targetAsset.terrainDetailLitShader
                //targetAsset.autodeskInteractiveMaskedShader
                //targetAsset.autodeskInteractiveTransparentShader
                //targetAsset.autodeskInteractiveShader
                //targetAsset.defaultMaterial
                //targetAsset.defaultShader
                //targetAsset.decalLayerNames
                //targetAsset.lightLayerNames
                //targetAsset.renderingLayerMaskNames
                //targetAsset.defaultMaterialQualityLevel
                //targetAsset.currentPlatformRenderPipelineSettings
                //targetAsset.defaultTerrainMaterial
                //targetAsset.virtualTexturingEnabled

                var sourceSettings = sourceAsset.currentPlatformRenderPipelineSettings;
                var targetSettings = targetAsset.currentPlatformRenderPipelineSettings;

                //sourceAsset.currentPlatformRenderPipelineSettings = targetAsset.currentPlatformRenderPipelineSettings;
            }
        }
    }
}
#endif
