using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class RefreshRateConnection : ConnectionWithOptions<string>
    {
        /// <summary>
        /// Disable if the refrehs rates change very often.
        /// </summary>
        public bool CacheRefreshRates = true;

        /// <summary>
        /// If enabled then only those refresh rates which are usable 
        /// with the current resolution are listed. That list may be
        /// much shorter than the full list (often just one).
        /// </summary>
        public bool LimitToCurrentResolution = false;

        public int MinRate = 0;
        public int MaxRate = 1000;

#if !UNITY_2022_2_OR_NEWER
        protected List<int> _values;
        protected List<string> _labels;
        protected string _rateNameInOptionLabel = "Hz";

        protected List<int> getRefreshRates()
        {
            if (_values == null || !CacheRefreshRates)
            {
                _values = new List<int>();
                _values.Add(Screen.currentResolution.refreshRate);

                var resolutions = Screen.resolutions;
                foreach (var res in resolutions)
                {
                    if (LimitToCurrentResolution && (res.width != Screen.currentResolution.width || res.height != Screen.currentResolution.height))
                        continue;

                    if (_values.Contains(res.refreshRate))
                        continue;

                    if (res.refreshRate < MinRate)
                        continue;

                    if (res.refreshRate > MaxRate)
                        continue;

                    _values.Add(res.refreshRate);
                }

                _values.Sort((a, b) => a - b);
            }

            return _values;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null || !CacheRefreshRates)
            {
                _labels = new List<string>();

                var refreshRates = getRefreshRates();
                foreach (var rate in refreshRates)
                {
                    string name = rate.ToString() + "" + _rateNameInOptionLabel;
                    _labels.Add(name);
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
            if (optionLabels == null || optionLabels.Count == 0)
                return;

            SetOptionLabel(optionLabels[0]);
            Logger.LogWarning("Setting each label name is not supported. Use SetOptionLabel() instead. Using the firast given as the new base label.");
        }

        public void SetOptionLabel(string rateNameInOptionLabel)
        {
            _rateNameInOptionLabel = rateNameInOptionLabel;
            RefreshOptionLabels();
        }

        protected int? lastKnownRefreshRate = null;
        protected int lastSetFrame = 0;

        public override int Get()
        {
            // Reset after N frames. The assumption is that
            // after that the Screen.currentResolution has been updated.
            if (Time.frameCount - lastSetFrame > 3)
                lastKnownRefreshRate = null;

            int currentRate = Screen.currentResolution.refreshRate;
            if (lastKnownRefreshRate.HasValue)
                currentRate = lastKnownRefreshRate.Value;

            var rates = getRefreshRates();
            for (int i = 0; i < rates.Count; i++)
            {
                if (rates[i] == currentRate)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// NOTICE: This has no effect in the Editor.<br />
        /// NOTICE: A resolution switch does not happen immediately; it happens when the current frame is finished.<br />
        /// See: https://docs.unity3d.com/ScriptReference/Screen.SetResolution.html
        /// </summary>
        /// <param name="index"></param>
        public override void Set(int index)
        {
            var refreshRates = getRefreshRates();
            index = Mathf.Clamp(index, 0, refreshRates.Count - 1);
            var rate = refreshRates[index];

            // Request change but delegate the actual execution to the orchestrator.
            ScreenOrchestrator.Instance.RequestRefreshRate(rate);

            // remember
            lastSetFrame = Time.frameCount;
            lastKnownRefreshRate = rate;

            NotifyListenersIfChanged(index);

#if UNITY_EDITOR
            if (SettingsGeneratorSettings.GetOrCreateSettings().ShowEditorInfoLogs)
            {
                Logger.LogMessage("Setting the refresh rate has no effect in the Editor. Please try in a build. - " + SettingsGeneratorSettings._showEditorInfoLogsHint);
            }
#endif
        }
#endif
    }
}
