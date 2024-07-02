// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
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
            if (_labels.IsNullOrEmpty())
            {
                _labels = QualitySettings.names.ToList();
            }
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != QualitySettings.names.Length)
            {
                Debug.LogError("Invalid new labels. Need to be " + QualitySettings.names.Length + ".");
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
            var distances = getDistances();

            for (int i = 0; i < distances.Count; i++)
            {
                if (distances[i] == QualitySettings.shadowDistance)
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
                // Generate list from QualitySettings if needed.
                if (_distancesFromSettings == null)
                {
                    _distancesFromSettings = new List<float>();
                    QualityPresets.AddAllLevels();
                    int levels = QualitySettings.names.Length;
                    for (int lvl = 0; lvl < levels; lvl++)
                    {
                        var preset = QualityPresets.GetPreset(lvl);
                        if (preset != null)
                            _distancesFromSettings.Add(preset.shadowDistance);
                    }
                }
                return _distancesFromSettings;
            }
        }

        public override void Set(int index)
        {
            var distances = getDistances();
            if (distances != null && distances.Count > index)
            {
                QualitySettings.shadowDistance = distances[index];
            }

            NotifyListenersIfChanged(index);
        }
    }
}
#endif
