#if KAMGAM_RENDER_PIPELINE_URP

// This class is based on another class which is under public domain.
// Thanks to: https://gist.github.com/JimmyCushnie/e998cdec15394d6b68a4dbbf700f66ce
// See: https://forum.unity.com/threads/change-shadow-resolution-from-script.784793/
// And also thanks to: https://forum.unity.com/threads/enable-or-disable-render-features-at-runtime.932571/

using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Kamgam.SettingsGenerator
{
    public static class UniversalRenderPipelineUtils
    {
        private static FieldInfo MainLightCastShadows_FieldInfo;
        private static FieldInfo AdditionalLightCastShadows_FieldInfo;
        private static FieldInfo MainLightShadowmapResolution_FieldInfo;
        private static FieldInfo AdditionalLightShadowmapResolution_FieldInfo;
        private static FieldInfo Cascade2Split_FieldInfo;
        private static FieldInfo Cascade4Split_FieldInfo;
        private static FieldInfo SoftShadowsEnabled_FieldInfo;
        private static FieldInfo RenderDataList_FieldInfo;

        static UniversalRenderPipelineUtils()
        {
            try
            {
                var pipelineAssetType = typeof(UniversalRenderPipelineAsset);
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;

                MainLightCastShadows_FieldInfo = pipelineAssetType.GetField("m_MainLightShadowsSupported", flags);
                AdditionalLightCastShadows_FieldInfo = pipelineAssetType.GetField("m_AdditionalLightShadowsSupported", flags);
                MainLightShadowmapResolution_FieldInfo = pipelineAssetType.GetField("m_MainLightShadowmapResolution", flags);
                AdditionalLightShadowmapResolution_FieldInfo = pipelineAssetType.GetField("m_AdditionalLightsShadowmapResolution", flags);
                Cascade2Split_FieldInfo = pipelineAssetType.GetField("m_Cascade2Split", flags);
                Cascade4Split_FieldInfo = pipelineAssetType.GetField("m_Cascade4Split", flags);
                SoftShadowsEnabled_FieldInfo = pipelineAssetType.GetField("m_SoftShadowsSupported", flags);
                RenderDataList_FieldInfo = pipelineAssetType.GetField("m_RendererDataList", flags);
            }
            catch (System.Exception e)
            {
                Debug.LogError("UniversalRenderPipelineUtils reflection cache build failed. Maybe the API has changed? \n" + e.Message);
            }
        }

        public static void SetMainLightCastShadows(bool value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null) {
                asset = (UniversalRenderPipelineAsset) GraphicsSettings.currentRenderPipeline;
            }

            if (MainLightCastShadows_FieldInfo != null) {
                MainLightCastShadows_FieldInfo.SetValue(asset, value);
            }
        }

        public static void SetAdditionalLightCastShadows(bool value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset) GraphicsSettings.currentRenderPipeline;
            }

            if (AdditionalLightCastShadows_FieldInfo != null)
            {
                AdditionalLightCastShadows_FieldInfo.SetValue(asset, value);
            }
        }

        public static void SetMainLightShadowResolution(int value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            }

            if (MainLightShadowmapResolution_FieldInfo != null)
            {
                MainLightShadowmapResolution_FieldInfo.SetValue(asset, value);
            }
        }

        public static void SetAdditionalLightShadowResolution(int value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            }

            if (AdditionalLightShadowmapResolution_FieldInfo != null)
            {
                AdditionalLightShadowmapResolution_FieldInfo.SetValue(asset, value);
            }
        }

        public static void SetCascade2Split(float value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            }

            if (Cascade2Split_FieldInfo != null)
            {
                Cascade2Split_FieldInfo.SetValue(asset, value);
            }
        }

        public static void SetCascade4Split(Vector3 value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            }

            if (Cascade4Split_FieldInfo != null)
            {
                Cascade4Split_FieldInfo.SetValue(asset, value);
            }
        }

        public static void SetSoftShadowsEnabled(bool value, UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            }

            if (SoftShadowsEnabled_FieldInfo != null)
            {
                SoftShadowsEnabled_FieldInfo.SetValue(asset, value);
            }
        }


        public static ScriptableRendererData[] GetRendererDataList(UniversalRenderPipelineAsset asset = null)
        {
            try
            {
                if (asset == null)
                {
                    asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                }
                if (asset == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("GetRenderFeature() current renderpipleline is null.");
#endif
                    return null;
                }

                if (RenderDataList_FieldInfo == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("GetRenderFeature() reflection failed to get m_RendererDataList field.");
#endif
                    return null;
                }

                var renderDataList = (ScriptableRendererData[])RenderDataList_FieldInfo.GetValue(asset);
                return renderDataList;
            }
            catch
            { 
                // Fail silently if reflection failed.
                return null;
            }
        }

        public static T GetRendererFeature<T>(UniversalRenderPipelineAsset asset = null) where T : ScriptableRendererFeature
        {
            var renderDataList = GetRendererDataList(asset);
            if (renderDataList == null || renderDataList.Length == 0)
                return null;

            foreach (var renderData in renderDataList)
            {
                foreach (var rendererFeature in renderData.rendererFeatures)
                {
                    if (rendererFeature is T)
                    {
                        return rendererFeature as T;
                    }
                }
            }

            return null;
        }

        public static ScriptableRendererFeature GetRendererFeature(string typeName, UniversalRenderPipelineAsset asset = null)
        {
            var renderDataList = GetRendererDataList(asset);
            if (renderDataList == null || renderDataList.Length == 0)
                return null;

            foreach (var renderData in renderDataList)
            {
                foreach (var rendererFeature in renderData.rendererFeatures)
                {
                    if (rendererFeature == null)
                        continue;

                    if (rendererFeature.GetType().Name.Contains(typeName))
                    {
                        return rendererFeature;
                    }
                }
            }

            return null;
        }

        public static T GetRendererFeatureChild<T>(ScriptableRendererFeature feature, string fieldName, string subFieldName = null)
        {
            if (feature == null)
                return default(T);

            var fieldInfo = feature.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                var fieldValue = fieldInfo.GetValue(feature);
                if (fieldValue != null)
                {
                    if (string.IsNullOrEmpty(subFieldName))
                    {
                        if (fieldValue is T)
                            return (T)fieldValue;
                    }
                    else
                    {
                        var subFieldInfo = fieldValue.GetType().GetField(subFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                        if (subFieldInfo != null)
                        {
                            var subFieldValue = subFieldInfo.GetValue(fieldValue);
                            if (subFieldValue != null && subFieldValue is T)
                            {
                                return (T)subFieldValue;
                            }
                        }
                    }
                }
            }

            return default(T);
        }

        public static void SetRendererFeatureChild<T>(T value, ScriptableRendererFeature feature, string fieldName, string subFieldName = null)
        {
            if (feature == null)
                return;

            var fieldInfo = feature.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                if (string.IsNullOrEmpty(subFieldName))
                {
                    fieldInfo.SetValue(feature, value);
                }
                else
                {
                    var fieldValue = fieldInfo.GetValue(feature);
                    if (fieldValue != null)
                    {
                        var subFieldInfo = fieldValue.GetType().GetField(subFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                        if (subFieldInfo != null)
                        {
                            subFieldInfo.SetValue(fieldValue, value);
                        }
                    }
                }
            }
        }

        public static bool IsRendererFeatureActive<T>(UniversalRenderPipelineAsset asset = null, bool defaultValue = false) where T : ScriptableRendererFeature
        {
            var feature = GetRendererFeature<T>(asset);
            if (feature == null)
                return defaultValue;

            return feature.isActive;
        }

        public static bool IsRendererFeatureActive(string typeName, UniversalRenderPipelineAsset asset = null, bool defaultValue = false)
        {
            var feature = GetRendererFeature(typeName, asset);
            if (feature == null)
                return defaultValue;

            return feature.isActive;
        }

        public static void SetRendererFeatureActive<T>(bool active, UniversalRenderPipelineAsset asset = null) where T : ScriptableRendererFeature
        {
            var feature = GetRendererFeature<T>(asset);
            if (feature == null)
                return;

            feature.SetActive(active);
        }

        public static void SetRendererFeatureActive(string typeName, bool active, UniversalRenderPipelineAsset asset = null)
        {
            var feature = GetRendererFeature(typeName, asset);
            if (feature == null)
                return;

            feature.SetActive(active);
        }
    }
}

#endif