#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    // Render Asset based setting. This changes values in the render asset associated
    // with the current quality level. This means we need some extra code to revert
    // these changes in the Editor (see "Editor RenderPipelineAsset Revert" region below).

    public partial class ShadowDistanceConnection
    {
        protected List<float> _distancesFromSettings;
        protected List<string> _labels;

        public ShadowDistanceConnection(List<float> qualityDistances, bool useQualitySettingsAsFallback = true)
        {
            QualityDistances = qualityDistances;
            UseQualitySettingsAsFallback = useQualitySettingsAsFallback;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = QualitySettings.names.ToList();

                // Ensure High to Low sorting (seems to be the default in HDRP).
                if (QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                {
                    _labels.Reverse();
                }
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var values = getDistances();
            if (optionLabels == null || optionLabels.Count != values.Count)
            {
                Debug.LogError("Invalid new labels. Need to be " + values.Count + ".");
                return;
            }

            _labels = new List<string>(optionLabels);
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

            var distances = getDistances();

            for (int i = 0; i < distances.Count; i++)
            {
                if (distances[i] == rpAsset.shadowDistance)
                {
                    return i;
                }
            }

            // Should never happen
            return QualitySettings.GetQualityLevel();
        }

        private List<float> getDistances()
        {
            if (!UseQualitySettingsAsFallback || (QualityDistances != null && QualityDistances.Count > 0))
            {
                return QualityDistances;
            }
            else
            {
                // Generate list from RenderPipelineAssets if needed.
                if (_distancesFromSettings == null)
                {
                    _distancesFromSettings = new List<float>();
                    int levels = QualitySettings.names.Length;
                    for (int level = 0; level < levels; level++)
                    {
                        var asset = QualitySettings.GetRenderPipelineAssetAt(level) as UniversalRenderPipelineAsset;
                        _distancesFromSettings.Add(asset.shadowDistance);
                    }
                }
                return _distancesFromSettings;
            }
        }

        public override void Set(int index)
        {
            var rpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (rpAsset == null)
                return;

            var distances = getDistances();
            if (distances != null && distances.Count > 0)
            {
                if (distances.Count > index)
                {
                    rpAsset.shadowDistance = distances[index];
                }
                else
                {
                    rpAsset.shadowDistance = distances[distances.Count-1];
                }
            }

            NotifyListenersIfChanged(index);
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
            RenderPipelineRestoreEditorUtils.InitAfterDomainReload<UniversalRenderPipelineAsset,float>(
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