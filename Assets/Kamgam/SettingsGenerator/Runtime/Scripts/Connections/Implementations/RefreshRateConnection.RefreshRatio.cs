using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // Since Unity 2021.2 the refreshRate is represented as a ratio instead of an int.
    // See: https://docs.unity3d.com/2021.2/Documentation/ScriptReference/RefreshRate.html

    public partial class RefreshRateConnection : ConnectionWithOptions<string>
    {
#if UNITY_2022_2_OR_NEWER
        protected List<RefreshRate> _values;
        protected List<string> _labels;
        protected string _rateNameInOptionLabel = "Hz";

        protected List<RefreshRate> getRefreshRates()
        {
            if (_values == null)
            {
                _values = new List<RefreshRate>();
                _values.Add(Screen.currentResolution.refreshRateRatio);

                var resolutions = Screen.resolutions;
                foreach (var res in resolutions)
                {
                    if (LimitToCurrentResolution && (res.width != Screen.currentResolution.width || res.height != Screen.currentResolution.height))
                        continue;

                    if (contains(_values, res.refreshRateRatio))
                        continue;

                    if (res.refreshRateRatio.value < MinRate)
                        continue;

                    if (res.refreshRateRatio.value > MaxRate)
                        continue;

                    _values.Add(res.refreshRateRatio);
                }

                _values.Sort((a, b) => Mathf.RoundToInt((float)(a.value - b.value)));
            }

            return _values;
        }

        // It turns out that Equals() does not return true for some ratios in builds.
        // Theory: Maybe the denominators and enumerators differ in builds while the
        // resulting value remains the same.
        protected bool contains(List<RefreshRate> rates, RefreshRate rate)
        {
            if (rates == null || rates.Count == 0)
                return false;

            int roundedRate = Mathf.RoundToInt((float)rate.value);

            for (int i = 0; i < rates.Count; i++)
            {
                int currentRoundedRate = Mathf.RoundToInt((float)rates[i].value);
                if (currentRoundedRate == roundedRate)
                    return true;
            }

            return false;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null || !CacheRefreshRates)
            {
                _labels = new List<string>();

                var refreshRates = getRefreshRates();
                foreach (var rate in refreshRates)
                {
                    string name = Mathf.RoundToInt((float)rate.value) + "" + _rateNameInOptionLabel;
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

        protected RefreshRate? lastKnownRefreshRate = null;
        protected int lastSetFrame = 0;

        public override int Get()
        {
            // Reset after N frames. The assumption is that
            // after that the Screen.currentResolution has been updated.
            if (Time.frameCount - lastSetFrame > 3)
                lastKnownRefreshRate = null;

            var currentRate = Screen.currentResolution.refreshRateRatio;
            if(lastKnownRefreshRate.HasValue)
                currentRate = lastKnownRefreshRate.Value;

            var rates = getRefreshRates();
            for (int i = 0; i < rates.Count; i++)
            {
                if (Mathf.Abs((float)(rates[i].value - currentRate.value)) < 0.01f)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// NOTICE: This has no effect in the Edtior.<br />
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
