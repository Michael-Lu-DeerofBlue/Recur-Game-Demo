#if KAMGAM_RENDER_PIPELINE_URP

using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Kamgam.SettingsGenerator
{
    public partial class QualityPreset
    {
        protected void applyToCurrentLevelURP()
        {
            var sourceAsset = (UniversalRenderPipelineAsset)renderPipeline;
            var targetAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;

            if (sourceAsset != null && targetAsset != null)
            {
                // Commented values are read only (those are not implemented yet)

                //targetAsset.defaultMaterial                       = sourceAsset.defaultMaterial;
                targetAsset.useAdaptivePerformance                = sourceAsset.useAdaptivePerformance;
                targetAsset.colorGradingLutSize                   = sourceAsset.colorGradingLutSize;
                targetAsset.colorGradingMode                      = sourceAsset.colorGradingMode;
                targetAsset.useSRPBatcher                         = sourceAsset.useSRPBatcher;
                //targetAsset.volumeFrameworkUpdateMode             = sourceAsset.volumeFrameworkUpdateMode;
                //targetAsset.defaultParticleMaterial               = sourceAsset.defaultParticleMaterial;
#if UNITY_2023_1_OR_NEWER
                // You thought shaderVariantLogLevel is still support, ha. Think again:
                // https://forum.unity.com/threads/deprecation-of-shadervariantloglevel.1415685/
#else
                targetAsset.shaderVariantLogLevel                 = sourceAsset.shaderVariantLogLevel;
#endif

                //targetAsset.supportsMixedLighting                 = sourceAsset.supportsMixedLighting;
                targetAsset.supportsDynamicBatching               = sourceAsset.supportsDynamicBatching;
                //targetAsset.supportsSoftShadows                   = sourceAsset.supportsSoftShadows;

                //targetAsset.defaultLineMaterial                   = sourceAsset.defaultLineMaterial;
                //targetAsset.defaultUIMaterial                     = sourceAsset.defaultUIMaterial;
                //targetAsset.shadowNormalBias                      = sourceAsset.shadowNormalBias;
                //targetAsset.defaultUIOverdrawMaterial             = sourceAsset.defaultUIOverdrawMaterial;
                //targetAsset.defaultUIETC1SupportedMaterial        = sourceAsset.defaultUIETC1SupportedMaterial;
                //targetAsset.default2DMaterial                     = sourceAsset.default2DMaterial;
                //targetAsset.defaultShader                         = sourceAsset.defaultShader;
                //targetAsset.autodeskInteractiveShader             = sourceAsset.autodeskInteractiveShader;
                //targetAsset.autodeskInteractiveTransparentShader  = sourceAsset.autodeskInteractiveTransparentShader;
                //targetAsset.autodeskInteractiveMaskedShader       = sourceAsset.autodeskInteractiveMaskedShader;
                //targetAsset.terrainDetailLitShader                = sourceAsset.terrainDetailLitShader;
                //targetAsset.terrainDetailGrassShader              = sourceAsset.terrainDetailGrassShader;
                //targetAsset.terrainDetailGrassBillboardShader     = sourceAsset.terrainDetailGrassBillboardShader;
                //targetAsset.defaultSpeedTree7Shader               = sourceAsset.defaultSpeedTree7Shader;
                //targetAsset.defaultTerrainMaterial                = sourceAsset.defaultTerrainMaterial;
                targetAsset.shadowDepthBias                       = sourceAsset.shadowDepthBias;
                //targetAsset.cascade2Split                         = sourceAsset.cascade2Split;
                //targetAsset.cascade3Split                         = sourceAsset.cascade3Split;
                //targetAsset.scriptableRenderer                    = sourceAsset.scriptableRenderer;
                targetAsset.supportsCameraDepthTexture            = sourceAsset.supportsCameraDepthTexture;
                targetAsset.supportsCameraOpaqueTexture           = sourceAsset.supportsCameraOpaqueTexture;
                //targetAsset.opaqueDownsampling                    = sourceAsset.opaqueDownsampling;
                //targetAsset.supportsTerrainHoles                  = sourceAsset.supportsTerrainHoles;
                //targetAsset.cascade4Split                         = sourceAsset.cascade4Split;
                targetAsset.supportsHDR                           = sourceAsset.supportsHDR;
                targetAsset.msaaSampleCount                       = sourceAsset.msaaSampleCount;
                targetAsset.renderScale                           = sourceAsset.renderScale;
                //targetAsset.storeActionsOptimization              = sourceAsset.storeActionsOptimization;
                //targetAsset.supportsMainLightShadows              = sourceAsset.supportsMainLightShadows;
                //targetAsset.mainLightRenderingMode                = sourceAsset.mainLightRenderingMode;
                //targetAsset.Shader defaultSpeedTree8Shader        = sourceAsset.Shader defaultSpeedTree8Shader;
                targetAsset.shadowCascadeCount                    = sourceAsset.shadowCascadeCount;
                targetAsset.shadowDistance                        = sourceAsset.shadowDistance;

                //targetAsset.supportsAdditionalLightShadows        = sourceAsset.supportsAdditionalLightShadows;
                targetAsset.maxAdditionalLightsCount              = sourceAsset.maxAdditionalLightsCount;
                //targetAsset.additionalLightsRenderingMode         = sourceAsset.additionalLightsRenderingMode;


                // These two we handle by using reflections
                //targetAsset.mainLightShadowmapResolution          = sourceAsset.mainLightShadowmapResolution;
                //targetAsset.additionalLightsShadowmapResolution   = sourceAsset.additionalLightsShadowmapResolution;
                // Sadly there is not setter for the shadow map resolutions. Thus we need
                // to use reflections to get it: https://forum.unity.com/threads/change-shadow-resolution-from-script.784793/#post-5447157
                try
                {
                    UniversalRenderPipelineUtils.SetMainLightShadowResolution(sourceAsset.mainLightShadowmapResolution, targetAsset);
                    UniversalRenderPipelineUtils.SetAdditionalLightShadowResolution(sourceAsset.additionalLightsShadowmapResolution, targetAsset);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("ShadowResolutionConnection reflection execution failed. Maybe the API has changed? \n" + e.Message);
                }
            }
        }
    }
}
#endif
