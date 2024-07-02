#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    // Render Asset based setting. This changes values in the render asset associated
    // with the current quality level. This means we need some extra code to revert
    // these changes in the Editor (see "Editor RenderPipelineAsset Revert" region below).

    // Maybe this could also be done by setting the MainLightCastShadows setting. Something to investigate.
    // See: https://forum.unity.com/threads/change-shadow-resolution-from-script.784793/

    public partial class ShadowConnection
    {
        /// <summary>
        /// Returns false if no data was found.
        /// </summary>
        /// <returns></returns>
        public override bool Get()
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            
            if(rpAsset == null)
                return false;

            remember();

            return rpAsset.shadowDistance > 0.001f ? true : false;
        }

        public override void Set(bool enable)
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (rpAsset == null)
                return;

            remember();

            if(enable)
            {
                revert();
            }
            else
            {
                rpAsset.shadowDistance = 0f;
            }

            NotifyListenersIfChanged(enable);
        }

        // One backup value per render pipeline asset.
        protected Dictionary<RenderPipelineAsset, float> previousValue;

        protected void remember()
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            // Skip if no asset was found.
            if (rpAsset == null)
                return;

            if (previousValue == null)
            {
                previousValue = new Dictionary<RenderPipelineAsset, float>();
            }

            // Add
            if (!previousValue.ContainsKey(GraphicsSettings.currentRenderPipeline))
            {
                previousValue.Add(GraphicsSettings.currentRenderPipeline, rpAsset.shadowDistance);
            }
            else
            {
                // Update only if it is not OFF (aka 0f)
                if (rpAsset.shadowDistance > 0.01f)
                {
                    previousValue[GraphicsSettings.currentRenderPipeline] = rpAsset.shadowDistance;
                }
            }
        }

        protected void revert()
        {
            foreach (var kv in previousValue)
            {
                if (kv.Key == null)
                    continue;

                if ((kv.Key as UniversalRenderPipelineAsset).shadowDistance < 0.001f)
                {
                    (kv.Key as UniversalRenderPipelineAsset).shadowDistance = kv.Value;
                }
            }
        }


        // Changes to Assets in the Editor are persistent, thus we have to
        // to revert them when leaving play mode.
#region Editor RenderPipelineAsset Revert

#if UNITY_EDITOR
        // One value per asset (low, mid high, ...)
        protected static Dictionary<RenderPipelineAsset, float> backupValues;

        [InitializeOnLoadMethod]
        protected static void initAfterDomainReload()
        {
            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset, float>(
                ref backupValues,
                (rpAsset) => rpAsset.shadowDistance,
                onPlayModeExit
            );
        }

        protected static void onPlayModeExit(PlayModeStateChange state)
        {
            RenderPipelineRestoreEditorUtils.OnPlayModeExit<UniversalRenderPipelineAsset, float>(
                state,
                backupValues,
                (rpAsset, value) => rpAsset.shadowDistance = value
            );
        }
#endif

#endregion

    }
}

#endif