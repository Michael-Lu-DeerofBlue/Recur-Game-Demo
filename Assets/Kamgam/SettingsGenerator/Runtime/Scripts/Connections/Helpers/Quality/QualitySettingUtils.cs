using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class QualitySettingUtils
    {
        static bool? _areQualitiesOrderedLowToHigh;

        /// <summary>
        /// Returns whether the order is LOW to HIGH (true) or HIGH to LOW (false).<br />
        /// <br />
        /// For some reason the default order in the HDRP template is high to low.<br />
        /// In all other templates it is low to high. So let's try to guess if they are reversed.<br />
        ///<br />
        /// Yes this is ridiculous but there no hard rules for these.<br />
        /// In theory the user could name and order them any way they like.
        /// </summary>
        /// <returns></returns>
        public static bool AreQualitiesOrderedLowToHigh()
        {
            if (_areQualitiesOrderedLowToHigh.HasValue)
                return _areQualitiesOrderedLowToHigh.Value;

            var names = QualitySettings.names;
            if (names != null && names.Length > 0)
            {
                string first = names[0];
                string last = names[names.Length - 1];
                if (first.Contains("High") || first.Contains("Best") || first.Contains("Ultra") ||
                    last.Contains("Low") || last.Contains("Bad") || last.Contains("Worst")
                    )
                {
                    _areQualitiesOrderedLowToHigh = false;
                    return false;
                }
            }

            // By default we assume low to high (that's the default in most unity versions).
            _areQualitiesOrderedLowToHigh = true;
            return true;
        }

        public static int MapToQualityLevel(int value, int min, int max)
        {
            int qualities = QualitySettings.names.Length-1;
            if(qualities == 0)
            {
                return 0;
            }

            float t = (value-min) / (float)(max - min);
            int mappedValue = Mathf.RoundToInt(qualities * t);
            return mappedValue;
        }

        public static int InvertQualityLevel(int qualityLevel)
        {
            int qualities = QualitySettings.names.Length-1;
            return qualities - qualityLevel;
        }

        public static int MapQualityLevelToRange(int qualityLevel, int min, int max)
        {
            int qualities = QualitySettings.names.Length-1;
            if (qualities == 0)
            {
                return min;
            }

            float level = qualityLevel;
            int mappedValue = Mathf.RoundToInt(min + ((max - min) * (level / qualities)));
            return mappedValue;
        }
    }
}
