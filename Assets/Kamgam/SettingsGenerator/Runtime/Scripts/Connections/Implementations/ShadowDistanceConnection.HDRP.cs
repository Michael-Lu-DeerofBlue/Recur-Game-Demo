#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // If you want to apply the setting to all cameras then you might want to
    // use Camera.onPreRender to check for new cameras and apply the changes.

    public partial class ShadowDistanceConnection
    {
        // Volume based setting. This means it will generate a volume game object
        // with a high priority to override the default settings made by other volumes.

        // This could also be implemented as a by-camera setting using the FrameSettingsField.ShadowMaps
        // of the camera HD data.

        // In HDRP there are no distances defined for each quality (or I have not
        // found one). Instead we try to get the max distance from the global volume.
        // But event that may fail and thus these fallback value for the min/max
        // distance exist.
        public static float ShadowDistanceMinHDRP = 50f;
        public static float ShadowDistanceMaxHDRP = 150f;

        protected SettingsVolumeShadowControlHDRP settingsVolumeShadowControlHDRP;

        protected List<float> _distances;
        protected List<string> _labels;

        protected bool _initialSetDone = false;

        public ShadowDistanceConnection(List<float> qualityDistances, bool useQualitySettingsAsFallback = true)
        {
            QualityDistances = qualityDistances;
            UseQualitySettingsAsFallback = useQualitySettingsAsFallback;
            getDistances(); // Automatically initializes the distances.
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

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
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

        protected void createShadowControlIfNeeded()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            if (settingsVolumeShadowControlHDRP == null)
            {
                settingsVolumeShadowControlHDRP = SettingsVolume.Instance.GetOrCreateControl<SettingsVolumeShadowControlHDRP>();
            }
        }

        public override int Get()
        {
            createShadowControlIfNeeded();
            var distances = getDistances();

            // Before the first SEt() we return an index based on the quality level.
            // Because otherwise the returned value would be based on the resolution
            // setting on a light source (these are not changed initially by quality
            // settings). This behaviour is more what people would
            // expect (low quality = low resolution).
            if (!_initialSetDone)
            {
                // No lights? Then fall back to last known value.
                int index = QualitySettingUtils.MapQualityLevelToRange(QualitySettings.GetQualityLevel(), 0, distances.Count - 1);
                return index;
            }

            if (distances != null && distances.Count > 0)
            {
                for (int i = 0; i < distances.Count; i++)
                {
                    if (distances[i] == settingsVolumeShadowControlHDRP.ShadowDistance)
                    {
                        return i;
                    }
                }
            }

            // return this if the distances are empty
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
                createShadowControlIfNeeded();

                // In HDRP we can not generate from the RenderPipelineAsset since there
                // is no shadow distance per quality in HDRP (or I have not found one).
                // Instead we try to find the "default" and calculate some
                // quality values from there.
                if (_distances == null)
                {
                    int levels = QualitySettings.names.Length;

                    // Start with fallback values
                    float max = ShadowDistanceMaxHDRP;
                    float min = ShadowDistanceMinHDRP;

                    // If a max value was found the derive min/max from that.
                    float? defaultMax = settingsVolumeShadowControlHDRP.DefaultShadowDistance;
                    if(defaultMax.HasValue)
                    {
                        min = defaultMax.Value / levels;
                        max = defaultMax.Value;
                    }

                    _distances = new List<float>();
                    if (levels <= 1)
                    {
                        _distances.Add(max);
                    }
                    else
                    {
                        // Spread the distances evenly from min to max.
                        float distancePerLevel = (max - min) / (levels - 1);
                        for (int level = 0; level < levels; level++)
                        {
                            float distance = ShadowDistanceMinHDRP + distancePerLevel * level;
                            _distances.Add(distance);
                        }

                        // Ensure High to Low sorting (seems to be the default in HDRP).
                        if (!QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                            _distances.Reverse();
                    }
                }
                return _distances;
            }
        }

        public override void Set(int index)
        {
            _initialSetDone = true;

            createShadowControlIfNeeded();

            var distances = getDistances();
            if (distances != null && distances.Count > 0)
            {
                if (distances.Count > index)
                {
                    settingsVolumeShadowControlHDRP.ShadowDistance = distances[index];
                }
                else
                {
                    settingsVolumeShadowControlHDRP.ShadowDistance = distances[distances.Count-1];
                }
            }

            NotifyListenersIfChanged(index);
        }

        public override void OnQualityChanged(int qualityLevel)
        {
            base.OnQualityChanged(qualityLevel);

            int optionCount = getDistances().Count;
            int index = QualitySettingUtils.MapQualityLevelToRange(qualityLevel, 0, optionCount-1);
            Set(index);
        }
    }
}

#endif
