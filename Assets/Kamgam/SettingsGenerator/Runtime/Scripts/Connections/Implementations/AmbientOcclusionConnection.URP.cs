#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    public partial class AmbientOcclusionConnection : Connection<bool>
    {
        /// <summary>
        /// Using the active state has the disadvantage that the functionality is dependent
        /// on the "disabled" shader variants. These may be stripped from builds.
        /// If you turn this on then you should disable shader stripping under
        /// ProjectSettings > Graphics > URP Global Settings > Shader Stripping (both "post pro" and "unused").
        /// See: https://forum.unity.com/threads/turn-urp-ssao-on-and-off-at-runtime.1066961/#post-8613702
        /// </summary>
        public static bool UseActiveStateToDisable = false;

        protected Dictionary<UniversalRenderPipelineAsset, float> _lastKnownIntensities = new Dictionary<UniversalRenderPipelineAsset, float>();

        // Renderer Asset based setting. This changes values in the render asset (ForwardRendererData.asset).
        // This means we need some extra code to revert these changes in the Editor (see
        // "EditorRenderPipelineAsset Revert" region below).

        protected ScriptableRenderer getRenderer()
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (rpAsset != null)
            {
                var renderer = rpAsset.scriptableRenderer;
                if (renderer != null)
                    return renderer;
            }

            return null;
        }

        static float getIntensity(UniversalRenderPipelineAsset rpAsset)
        {
            if (rpAsset == null)
                return 0f;
            var feature = UniversalRenderPipelineUtils.GetRendererFeature("ScreenSpaceAmbientOcclusion", rpAsset);
            return UniversalRenderPipelineUtils.GetRendererFeatureChild<float>(feature, "m_Settings", "Intensity");
        }

        static void setIntensity(UniversalRenderPipelineAsset rpAsset, float intensity)
        {
            if (rpAsset == null)
                return;
            var feature = UniversalRenderPipelineUtils.GetRendererFeature("ScreenSpaceAmbientOcclusion", rpAsset);
            UniversalRenderPipelineUtils.SetRendererFeatureChild(intensity, feature, "m_Settings", "Intensity");
        }

        protected void updateLastKnownIntensity(UniversalRenderPipelineAsset rpAsset)
        {
            if (!_lastKnownIntensities.ContainsKey(rpAsset))
            {
                _lastKnownIntensities.Add(rpAsset, getIntensity(rpAsset));
            }
        }

        /// <summary>
        /// Returns false if no data was found.
        /// </summary>
        /// <returns></returns>
        public override bool Get()
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (rpAsset == null)
            {
                return false;
            }

            // Yeah, accessing via strings sucks, but "this is the way" in Unity it seems.
            // There is a generic GetRendererFeature<T> too but the ScreenSpaceAmbientOcclusion
            // type is declared as "internal" by Unity, thus we can not use it that way *sigh*.
            var feature = UniversalRenderPipelineUtils.GetRendererFeature("ScreenSpaceAmbientOcclusion", rpAsset);

            if (UseActiveStateToDisable)
            {
                return feature.isActive;
            }
            else
            {
                updateLastKnownIntensity(rpAsset);
                float intensity = getIntensity(rpAsset);
                return intensity > 0.001f;
            }
        }

        public override void Set(bool enable)
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (rpAsset == null)
            {
                return;
            }

            var feature = UniversalRenderPipelineUtils.GetRendererFeature("ScreenSpaceAmbientOcclusion", rpAsset);
            if (feature != null)
            {
                if (UseActiveStateToDisable)
                {
                    feature.SetActive(enable);
                }
                else
                {
                    updateLastKnownIntensity(rpAsset);
                    float intensity = enable ? _lastKnownIntensities[rpAsset] : 0.001f;
                    setIntensity(rpAsset, intensity);
                }
            }

            NotifyListenersIfChanged(enable);

#if UNITY_EDITOR
            if (feature == null)
            {
                Logger.Log("AmbientOcclusionConnection: No ScreenSpaceAmbientOcclusion feature found on the current renderer. This settings will have no effect. Please look here for guidance on how to add one: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@15.0/manual/urp-renderer-feature-how-to-add.html");
            }
#endif
        }

        // Changes to Assets in the Editor are persistent, thus we have to
        // to revert them when leaving play mode.
#region Editor RenderPipelineAsset Revert

#if UNITY_EDITOR
        // One value per asset (low, mid high, ...)
        protected static Dictionary<RenderPipelineAsset, bool> backupValues;
        protected static Dictionary<RenderPipelineAsset, float> backupIntensityValues;

        [InitializeOnLoadMethod]
        protected static void initAfterDomainReload()
        {
            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset, bool>(
                ref backupValues,
                (rpAsset) => UniversalRenderPipelineUtils.IsRendererFeatureActive("ScreenSpaceAmbientOcclusion"),
                onPlayModeExit
            );

            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset, float>(
                ref backupIntensityValues,
                (rpAsset) => getIntensity(rpAsset),
                null
            );
        }

        protected static void onPlayModeExit(PlayModeStateChange state)
        {
            RenderPipelineRestoreEditorUtils.OnPlayModeExit<UniversalRenderPipelineAsset, bool>(
                state,
                backupValues,
                (rpAsset, value) => UniversalRenderPipelineUtils.SetRendererFeatureActive("ScreenSpaceAmbientOcclusion", value)
            );

            RenderPipelineRestoreEditorUtils.OnPlayModeExit<UniversalRenderPipelineAsset, float>(
                state,
                backupIntensityValues,
                (rpAsset, value) => setIntensity(rpAsset, value)
            );
        }
#endif

#endregion
    }
}

#endif