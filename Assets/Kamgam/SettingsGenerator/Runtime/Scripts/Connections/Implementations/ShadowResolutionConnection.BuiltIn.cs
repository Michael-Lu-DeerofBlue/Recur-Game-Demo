// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class ShadowResolutionConnection
    {
        protected List<string> _labels;
        protected List<ShadowResolution> _values;

        protected List<ShadowResolution> getValues()
        {
            if (_values.IsNullOrEmpty())
            {
                _values = new List<ShadowResolution>();
                _values.Add(ShadowResolution.Low);
                _values.Add(ShadowResolution.Medium);
                _values.Add(ShadowResolution.High);
                _values.Add(ShadowResolution.VeryHigh);
            }

            return _values;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels.IsNullOrEmpty())
            {
                _labels = new List<string>();

                _labels.Add("Low");
                _labels.Add("Medium");
                _labels.Add("High");
                _labels.Add("Very High");
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var values = getValues();
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
            var optionValues = getValues();
            for (int i = 0; i < optionValues.Count; i++)
            {
                if (optionValues[i] == QualitySettings.shadowResolution)
                {
                    return i;
                }
            }

            return 0;
        }

        public override void Set(int index)
        {
            var optionValues = getValues();
            index = Mathf.Clamp(index, 0, optionValues.Count - 1);
            var value = optionValues[index];

            QualitySettings.shadowResolution = value;

            NotifyListenersIfChanged(index);
        }
    }
}
#endif
