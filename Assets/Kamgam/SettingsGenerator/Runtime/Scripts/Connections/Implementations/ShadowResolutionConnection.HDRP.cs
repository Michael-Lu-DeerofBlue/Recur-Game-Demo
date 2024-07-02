#if KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP

using System.Collections.Generic;
// using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Kamgam.SettingsGenerator
{
    public partial class ShadowResolutionConnection
    {
        protected List<int> _values;
        protected List<string> _labels;

        protected bool _initialSetDone = false;
        protected bool _overrideCustomShadowResolution = true;

        /// <summary>
        /// Shadow Resolution connection controlling the shadow resolution on every light.
        /// </summary>
        public ShadowResolutionConnection()
        {
            lastKnownValue = Get();
            LightDetector.Instance.OnNewLightFound += onNewLightFound;
        }

        /// <summary>
        /// If a light has a custom shadow map resolution set
        /// instead of a level then this defines whether or not
        /// to override the custom resolution.
        /// <br /><br />
        /// NOTICE: If you uncheck this and you have a light with a
        ///  custom shadow map resolution set then the 'ShadowResolution'
        ///  setting will have no effect on that light.
        /// </summary>
        /// <param name="override"></param>
        public void SetOverrideCustomShadowResolution(bool @override)
        {
            _overrideCustomShadowResolution = @override;
        }

        protected void onNewLightFound(Light light)
        {
            applyToLight(light, lastKnownValue);
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = new List<string>()
                {
                    "Ultra", "High", "Mid", "Low"
                };

                // Ensure High to Low sorting (seems to be the default in HDRP).
                if (QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                    _labels.Reverse();
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var values = getResolutions();
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
            var resolutions = getResolutions();

            // Before the first Set() we return an index based on the quality level.
            // Because otherwise the returned value would be based on the resolution
            // setting on a light source (these are not changed initially by quality
            // settings). This behaviour is more what people would
            // expect (low quality = low resolution).
            if (!_initialSetDone)
            {
                // No lights? Then fall back to last known value.
                int index = QualitySettingUtils.MapQualityLevelToRange(QualitySettings.GetQualityLevel(), 0, resolutions.Count - 1);
                return index;
            }

            // Get level from the first light.
            var light = LightDetector.Instance.GetPrimaryLight();
            if (light != null)
            {
                var data = light.GetComponent<HDAdditionalLightData>();
                if (data != null)
                {
                    if (QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                    {
                        return data.shadowResolution.level;
                    }
                    else
                    {
                        return resolutions.Count - 1 - data.shadowResolution.level;
                    }
                }
            }

            return lastKnownValue;
        }

        private List<int> getResolutions()
        {
            if (_values == null)
            {
                _values = new List<int>()
                {
                    (int)ShadowResolution.VeryHigh,
                    (int)ShadowResolution.High,
                    (int)ShadowResolution.Medium,
                    (int)ShadowResolution.Low
                };

                // Ensure High to Low sorting (seems to be the default in HDRP).
                if (QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                    _values.Reverse();
            }

            return _values;
        }

        public override void Set(int index)
        {
            _initialSetDone = true;

            var resolutions = getResolutions();
            var resolutionIndex = resolutions[index];

            // Update all the lights
            var lights = LightDetector.Instance.Lights;
            foreach (var light in lights)
            {
                if (light != null)
                {
                    applyToLight(light, resolutionIndex);
                }
            }

            NotifyListenersIfChanged(index);
        }

        protected void applyToLight(Light light, int index)
        {
            var data = light.GetComponent<HDAdditionalLightData>();
            if (data == null)
                return;

            // disable custom resolution?
            bool usesCustomResolution = data.shadowResolution.useOverride;
            if (usesCustomResolution && _overrideCustomShadowResolution)
            {
                data.SetShadowResolutionOverride(false);
            }

            // set level
            data.SetShadowResolutionLevel(index);
        }

        public override void OnQualityChanged(int qualityLevel)
        {
            base.OnQualityChanged(qualityLevel);

            int optionCount = getResolutions().Count;
            int index = QualitySettingUtils.MapQualityLevelToRange(qualityLevel, 0, optionCount-1);
            Set(index);
        }
    }
}

#endif
