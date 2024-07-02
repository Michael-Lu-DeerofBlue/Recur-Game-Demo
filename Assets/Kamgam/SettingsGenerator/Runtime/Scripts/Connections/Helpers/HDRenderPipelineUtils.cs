#if KAMGAM_RENDER_PIPELINE_HDRP

using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public static class HDRenderPipelineUtils
    {
        private static FieldInfo RenderPipelineSettings_FieldInfo;

        static HDRenderPipelineUtils()
        {
            try
            {
                var pipelineAssetType = typeof(HDRenderPipelineAsset);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;

                RenderPipelineSettings_FieldInfo = pipelineAssetType.GetField("m_RenderPipelineSettings", flags);
            }
            catch (System.Exception e)
            {
                Debug.LogError("HDRenderPipelineUtils reflection cache build failed. Maybe the API has changed? \n" + e.Message);
            }
        }

        public static void SetRenderPipelineSettings(RenderPipelineSettings settings, HDRenderPipelineAsset asset = null)
        {
            if (asset == null) {
                asset = (HDRenderPipelineAsset) GraphicsSettings.currentRenderPipeline;
            }

            if (RenderPipelineSettings_FieldInfo != null) {
                // Sadly this does not seem to work. The value gets updated but the
                // change is not becoming visible (probably some update is needed).
                // TODO: investigate.
                RenderPipelineSettings_FieldInfo.SetValue(asset, settings);
            }
        }
    }
}

#endif