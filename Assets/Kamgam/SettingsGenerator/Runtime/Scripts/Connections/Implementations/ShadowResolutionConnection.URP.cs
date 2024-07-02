#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    // Render Asset based setting. This changes values in the render asset associated
    // with the current quality level. This means we need some extra code to revert
    // these changes in the Editor (see "Editor RenderPipelineAsset Revert" region below).

    public partial class ShadowResolutionConnection
    {
        /// <summary>
        /// Should the resolution of the addition lights be changed too?<br />
        /// If yes then it is changed according to the 'AdditionalToMainResolutionFactor' (main resolution divided by that factor).
        /// </summary>
        public static bool SetAdditionalLightResolution = true;

        /// <summary>
        /// If 'SetAdditionalLightResolution' is true then additional light shadow resolutions are set the main resolution divided by this factor.
        /// </summary>
        public static int AdditionalToMainResolutionFactor = 4;

        protected List<int> _values;
        protected List<string> _labels;

        static void setResolution(UniversalRenderPipelineAsset asset, int resolution)
        {
            // Sadly there is not setter for the shadow map resolutions. Thus we need
            // to use reflections to get it: https://forum.unity.com/threads/change-shadow-resolution-from-script.784793/#post-5447157
            try
            {
                UniversalRenderPipelineUtils.SetMainLightShadowResolution(resolution, asset);
                if (SetAdditionalLightResolution)
                {
                    UniversalRenderPipelineUtils.SetAdditionalLightShadowResolution(
                        Mathf.Max(resolution / AdditionalToMainResolutionFactor, (int)UnityEngine.Rendering.Universal.ShadowResolution._256),
                        asset
                    );
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("ShadowResolutionConnection reflection execution failed. Maybe the API has changed? \n" + e.Message);
            }
        }

        public override List<string> GetOptionLabels()
        {
            if(_labels == null)
            {
                _labels = new List<string>
                {
                    "Low",
                    "Mid",
                    "High",
                    "Very High",
                    "Ultra"
                };
            }
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var optionValues = getResolutions();
            if (optionLabels == null || optionLabels.Count != optionValues.Count)
            {
                Debug.LogError("Invalid new labels. Need to be " + optionValues + ".");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override int Get()
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (rpAsset == null)
                return 0;

            var resolutions = getResolutions();

            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i] == rpAsset.mainLightShadowmapResolution)
                {
                    return i;
                }
            }

            // Should never happen
            return Mathf.Min(QualitySettings.GetQualityLevel(), resolutions.Count);
        }

        private List<int> getResolutions()
        {
            // Generate list from RenderPipelineAssets if needed.
            if (_values == null)
            {
                _values = new List<int>()
                {
                    (int)UnityEngine.Rendering.Universal.ShadowResolution._256,
                    (int)UnityEngine.Rendering.Universal.ShadowResolution._512,
                    (int)UnityEngine.Rendering.Universal.ShadowResolution._1024,
                    (int)UnityEngine.Rendering.Universal.ShadowResolution._2048,
                    (int)UnityEngine.Rendering.Universal.ShadowResolution._4096
                };
            }

            return _values;
        }

        public override void Set(int index)
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (rpAsset == null)
                return;

            var resolutions = getResolutions();
            if (resolutions != null && resolutions.Count > 0)
            {
                if (resolutions.Count > index)
                {
                    setResolution(rpAsset, resolutions[index]);
                }
                else
                {
                    setResolution(rpAsset, resolutions[resolutions.Count - 1]);
                }
            }

            NotifyListenersIfChanged(index);
        }

        // Changes to Assets in the Editor are persistent, thus we have to
        // to revert them when leaving play mode.
#region Editor RenderPipelineAsset Revert

#if UNITY_EDITOR
        // One value per asset (low, mid high, ...)
        protected static Dictionary<RenderPipelineAsset, int> mainResolutionValues;
        protected static Dictionary<RenderPipelineAsset, int> additionalResolutionValues;

        [InitializeOnLoadMethod]
        protected static void initAfterDomainReload()
        {
            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset,int>(
                ref mainResolutionValues,
                (rpAsset) => rpAsset.mainLightShadowmapResolution,
                onPlayModeExit
            );

            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset, int>(
                ref additionalResolutionValues,
                (rpAsset) => rpAsset.additionalLightsShadowmapResolution,
                null // the first call aready registers the callback, no need for another one
            );
        }

        protected static void onPlayModeExit(PlayModeStateChange state)
        {
            RenderPipelineRestoreEditorUtils.OnPlayModeExit<UniversalRenderPipelineAsset, int>(
                state,
                mainResolutionValues,
                (rpAsset, value) => UniversalRenderPipelineUtils.SetMainLightShadowResolution(value, rpAsset)
            );

            RenderPipelineRestoreEditorUtils.OnPlayModeExit<UniversalRenderPipelineAsset, int>(
                state,
                additionalResolutionValues,
                (rpAsset, value) => UniversalRenderPipelineUtils.SetAdditionalLightShadowResolution(value, rpAsset)
            );
        }
#endif

#endregion
    }
}

#endif