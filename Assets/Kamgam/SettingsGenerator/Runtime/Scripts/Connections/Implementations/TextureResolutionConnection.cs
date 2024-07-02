using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class TextureResolutionConnection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;
        protected List<int> _values;

        protected List<int> getValues()
        {
            if (_values.IsNullOrEmpty())
            {
                _values = new List<int>();
                _values.Add(0);
                _values.Add(1);
                _values.Add(2);
                _values.Add(3);

                if (QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                {
                    _values.Reverse();
                }
            }

            return _values;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels.IsNullOrEmpty())
            {
                _labels = new List<string>();

                _labels.Add("Full Resolution");
                _labels.Add("Half Resolution");
                _labels.Add("Quater Resolution");
                _labels.Add("Eighth Resolution");

                if (QualitySettingUtils.AreQualitiesOrderedLowToHigh())
                {
                    _labels.Reverse();
                }
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
#if UNITY_2022_2_OR_NEWER
                if (optionValues[i] == QualitySettings.globalTextureMipmapLimit)
#else
                if (optionValues[i] == QualitySettings.masterTextureLimit)
#endif
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

#if UNITY_2022_2_OR_NEWER
            QualitySettings.globalTextureMipmapLimit = value;
#else
            QualitySettings.masterTextureLimit = value;
#endif


            NotifyListenersIfChanged(index);
        }
    }
}
