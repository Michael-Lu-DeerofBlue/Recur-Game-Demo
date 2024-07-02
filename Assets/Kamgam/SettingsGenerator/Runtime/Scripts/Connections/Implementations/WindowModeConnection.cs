using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class WindowModeConnection : ConnectionWithOptions<string>
    {
        protected List<FullScreenMode> _values;
        protected List<string> _labels;

        protected FullScreenMode? lastKnownMode = null;
        protected int lastSetFrame = 0;

        public override List<string> GetOptionLabels()
        {
            if (_labels.IsNullOrEmpty())
            { 
                _labels = new List<string>();

                _labels.Add("Full Screen");
                _labels.Add("Window");
                _labels.Add("Exclusive (Windows)");
                _labels.Add("Maximized (MacOS)");
            }

            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            var values = getWindowOptions();
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

        protected List<FullScreenMode> getWindowOptions()
        {
            if (_values.IsNullOrEmpty())
            {
                _values = new List<FullScreenMode>();

                _values.Add(FullScreenMode.FullScreenWindow);
                _values.Add(FullScreenMode.Windowed);
                _values.Add(FullScreenMode.ExclusiveFullScreen);
                _values.Add(FullScreenMode.MaximizedWindow);
            }

            return _values;
        }

        /// <summary>
        /// Returns the selected index.
        /// </summary>
        /// <returns></returns>
        public override int Get()
        {
            // Reset after N frames. The assumption is that
            // after that the Screen.fullScreenMode has been updated.
            if (Time.frameCount - lastSetFrame > 3)
                lastKnownMode = null;

            FullScreenMode currentMode = Screen.fullScreenMode;
            if (lastKnownMode.HasValue)
            {
                currentMode = lastKnownMode.Value;
            }

            var option = getWindowOptions();
            for (int i = 0; i < option.Count; i++)
            {
                if (option[i] == currentMode)
                {
                    return i;
                }
            }

            return 0;
        }

        public override void Set(int index)
        {
            var options = getWindowOptions();
            index = Mathf.Clamp(index, 0, options.Count - 1);
            var mode = options[index];

            // Request change but delegate the actual execution to the orchestrator.
            ScreenOrchestrator.Instance.RequestFullScreenMode(mode);

            // Remember
            lastSetFrame = Time.frameCount;
            lastKnownMode = mode;

            NotifyListenersIfChanged(index);

#if UNITY_EDITOR
            if (SettingsGeneratorSettings.GetOrCreateSettings().ShowEditorInfoLogs)
            {
                Logger.LogMessage("Setting the WindowMode has no effect in the Editor. Please try in a build. - " + SettingsGeneratorSettings._showEditorInfoLogsHint);
            }
#endif
        }
    }
}
