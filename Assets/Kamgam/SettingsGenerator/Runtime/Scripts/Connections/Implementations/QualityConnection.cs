using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class QualityConnection : ConnectionWithOptions<string>, IConnectionWithSettingsAccess
    {
        public Settings Settings;

        protected List<string> _labels;
        protected List<int> _values;

        [System.Obsolete("QualityConnection(Settings settings) constuctor is deprecated. Use the default constructor and SetSettings(Settings settings) instead.")]
        public QualityConnection(Settings settings)
        {
            Settings = settings;
        }

        public QualityConnection()
        {
        }

        public override int GetOrder()
        {
            // Apply quality before other settings.
            // This way other settings can override the quality settings.
            // It's also important for Set() after the inital load (see comments below).
            return base.GetOrder() - 1;
        }

        public override int Get()
        {
            return QualitySettings.GetQualityLevel();
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = QualitySettings.names.ToList();
            }
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != QualitySettings.names.Length)
            {
                Debug.LogError("Invalid new labels for QualityConnection. Need to be " + QualitySettings.names.Length + ".");
                return;
            }

            _labels = new List<string>(optionLabels);
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override void Set(int value)
        {
            int currentQualityLevel = QualitySettings.GetQualityLevel();

            // Restore old
            QualityPresets.RestoreCurrentLevel();

            // Set and restore new
            QualitySettings.SetQualityLevel(value);
            QualityPresets.RestoreCurrentLevel();
            QualityPresets.AddCurrentLevel();

            // Inform settings of quality change
            Settings.OnQualityChanged(value, excludeChanged: true);

            // NOTICE: All settings start as "unapplied" after loading. Therefore
            // this will not pull any settings if the quality is set for the first time.
            // We actually want this because if not then this would overwrite all freshly
            // loaded settings. It should only overwrite the other settings if changed later.
            Settings.PullFromConnections(exceptUnapplied: true);
            Settings.RefreshRegisteredResolvers();

            NotifyListenersIfChanged(value);
        }

        public void SetSettings(Settings settings)
        {
            Settings = settings;
        }

        public Settings GetSettings()
        {
            return Settings;
        }
    }
}
