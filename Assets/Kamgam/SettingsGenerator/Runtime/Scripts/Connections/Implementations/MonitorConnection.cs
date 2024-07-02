using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MonitorConnection : ConnectionWithOptions<string>
    {
        /// <summary>
        /// It is not advisable to change the monitor on mobile.
        /// It may have unexpected side effects and there usually is
        /// just one anyways.
        /// </summary>
        public static bool AllowMonitorChangeOnMobile = false;

        /// <summary>
        /// Enable this if you are having difficulties with monitor settings not being set at the start.
        /// </summary>
        public static bool ForceMonitorUpdate = false;

        public static int FramesToWaitAfterMonitorSwitch = 3;

        /// <summary>
        /// These actions will be executed after the switch has completed.<br />
        /// Keep in mind that monitor switching is an async action so these actions
        /// are executed a few frames after Set() has been called.
        /// </summary>
#pragma warning disable CS0067 // The event is never used
        public event System.Action OnComplete;
#pragma warning restore CS0067

        /// <summary>
        /// If set to true then it will try to trigger a refresh of all resolvers depending on display settings.
        /// </summary>
        public bool RefreshResolversAfterCompletion = true;

#if !UNITY_2021_2_OR_NEWER
        // Unity versions below 2021.2 do not have the DisplayInfo and RefreshRate class, so we fake them.
        public class RefreshRate
        {
            public uint denominator = 60000;
            public uint numerator = 1001;
            public float value => denominator / numerator;

            static uint[] denNum = new uint[]
            {
                3000, 1000, // 30 Hz 
                60000, 1001, // 59.94 Hz 
                60000, 1000, // 60 Hz
                100000, 1000, // 100 Hz
                120000, 1000, // 120 Hz
                200000, 1000, // 200 Hz
                240000, 1000, // 240 Hz
                500000, 1000  // 500 Hz
            };

            public static RefreshRate FromHz(float hz)
            {
                var refreshRate = new RefreshRate();

                // find closes to the given Hz.
                float minDelta = float.MaxValue;
                uint foundDenominator = 60000;
                uint foundNumerator = 1001;
                float candidateHz;
                float delta;
                for (int i = 0; i < denNum.Length; i += 2)
                {
                    candidateHz = (float)denNum[i] / denNum[i + 1];
                    delta = Mathf.Abs(candidateHz - hz);
                    if (delta < minDelta)
                    {
                        minDelta = delta;
                        foundDenominator = denNum[i];
                        foundNumerator = denNum[i + 1];
                    }
                }

                refreshRate.denominator = foundDenominator;
                refreshRate.numerator = foundNumerator;

                return refreshRate;
            }
        }

        public struct DisplayInfo
        {
            public string name;
            public int width;
            public int height;
            public RefreshRate refreshRate;
            public RectInt workArea;
        }
#endif

        protected List<DisplayInfo> _values;
        protected List<string> _labels;

        protected List<DisplayInfo> getDisplayInfos()
        {
            if (_values.IsNullOrEmpty())
            {
                _values = new List<DisplayInfo>();

                // Generate a list of display infos
#if UNITY_2021_2_OR_NEWER
                Screen.GetDisplayLayout(_values);
#else
                for (int i = 0; i < Display.displays.Length; i++)
                {
                    var info = new DisplayInfo();
                    info.name = "Monitor " + (i + 1);
                    info.width = Display.displays[i].systemWidth;
                    info.height = Display.displays[i].systemHeight;
                    info.refreshRate = RefreshRate.FromHz(60f); // Have not found an API for that, so we assume 60
                    info.workArea = new RectInt(0, 0, info.width, info.height);
                    _values.Add(info);
                }
#endif

                // Hard fallback
                if (_values.Count == 0)
                {
#if UNITY_2021_2_OR_NEWER
                    var info = new DisplayInfo();
                    info.name = "Monitor 1";
                    info.width = 1920;
                    info.height = 1080;
                    info.refreshRate = new RefreshRate() { denominator = 60000, numerator = 1001 };
                    info.workArea = new RectInt(0, 0, info.width, info.height);
                    _values.Add(info);
#else
                    var info = new DisplayInfo();
                    info.name = "Monitor 1";
                    info.width = 1920;
                    info.height = 1080;
                    info.refreshRate = RefreshRate.FromHz(60f);
                    info.workArea = new RectInt(0, 0, info.width, info.height);
                    _values.Add(info);
#endif
                }
            }

            return _values;
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels.IsNullOrEmpty())
            {
                _labels = new List<string>();

                var displays = getDisplayInfos();
                foreach (var info in displays)
                {
                    string name = info.name + " (" + info.width + "x" + info.height + ")";
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
            var resolutions = getDisplayInfos();
            if (optionLabels == null || optionLabels.Count != resolutions.Count)
            {
                Logger.LogError("Invalid new labels. Need to be " + resolutions.Count + ".");
            }

            _labels = new List<string>(optionLabels);
        }

#if UNITY_2021_2_OR_NEWER
        protected int _lastKnownMonitorIndex = -1;
        protected int _lastSetFrame = -1;
#endif

        public override int Get()
        {
#if UNITY_2021_2_OR_NEWER
            // Clean up after the operation has been done.
            // Wait at least N frames to give unity time to catch up to the change.
            // Yes, this is necessary even if we wait for _moveOperation to finish.
            if (((_moveOperation != null && _moveOperation.isDone) || _moveOperationFailed) && Time.frameCount - _lastSetFrame > FramesToWaitAfterMonitorSwitch)
            {
                _lastKnownMonitorIndex = -1;
                _lastSetFrame = -1;
                _moveOperation = null;
                _moveOperationFailed = false;
            }

            // Return the last set value if the swiching is not done yet.
            if (_lastKnownMonitorIndex >= 0)
            {
                return _lastKnownMonitorIndex;
            }

            // Get monitor index
            var infos = getDisplayInfos();
            return infos.IndexOf(Screen.mainWindowDisplayInfo);
#else
            // Not supported
            Logger.LogWarning(
                "Sorry, the monitor switching API is only available in Uniy 2021.2 or newer. This is a Unity limitation an can not be changed." +
                "On Windows you might be able to implement something with the Win32 native API, see here: https://forum.unity.com/threads/switch-monitor-at-runtime.501336/#post-6882695");

            return 0;
#endif
        }

        /// <summary>
        /// NOTICE: This has no effect in the Edtior.<br />
        /// NOTICE: A monitor switch does not happen immediately; it may take multiple frames.<br />
        /// See: https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Screen.MoveMainWindowTo.html
        /// </summary>
        /// <param name="index"></param>
        public override void Set(int index)
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_SWITCH
            if (!AllowMonitorChangeOnMobile)
            {
                Debug.LogWarning("Allow monitor change on mobile is disabled. It is not advisable to change the monitor on mobile. It may have unexpected sideeffects and there usually is just one anyways.");
                return;
            }
#endif

#if UNITY_2021_2_OR_NEWER
            _lastSetFrame = Time.frameCount;
            _lastKnownMonitorIndex = index;
            moveToMonitor(index);
            NotifyListenersIfChanged(index);
#else
            Logger.LogWarning(
                "Sorry, the monitor switching API is only available in Unity 2021.2 or newer. This is a Unity limitation an can not be changed." +
                "On Windows you might be able to implement something with the Win32 native API, see here: https://forum.unity.com/threads/switch-monitor-at-runtime.501336/#post-6882695");
#endif

#if UNITY_EDITOR
            if (SettingsGeneratorSettings.GetOrCreateSettings().ShowEditorInfoLogs)
            {
                Logger.LogMessage("Setting the monitor has no effect in the Editor. Please try in a build. - " + SettingsGeneratorSettings._showEditorInfoLogsHint);
            }
#endif
        }

#if UNITY_2021_2_OR_NEWER
        protected AsyncOperation _moveOperation;
        protected bool _moveOperationFailed;

        void moveToMonitor(int index)
        {
            try
            {
                _moveOperationFailed = false;
                var display = _values[index];

                Vector2Int targetCoordinates = Screen.mainWindowPosition;
                
                if (Screen.fullScreenMode != FullScreenMode.Windowed)
                {
                    // Target the center of the display. Doing it this way shows off
                    // that MoveMainWindow snaps the window to the top left corner
                    // of the display when running in fullscreen mode.
                    targetCoordinates.x += display.width / 2;
                    targetCoordinates.y += display.height / 2;
                }

                // Only update if a monitor switch is necessary
                if (Screen.mainWindowDisplayInfo.name != display.name || ForceMonitorUpdate)
                {
                    _moveOperation = Screen.MoveMainWindowTo(display, targetCoordinates);
                    waitForMonitorSwitchToComplete();
                }
            }
            catch
            {
                _moveOperationFailed = true;
            }
        }

        async void waitForMonitorSwitchToComplete()
        {
            while(!_moveOperation.isDone || Time.frameCount - _lastSetFrame <= FramesToWaitAfterMonitorSwitch)
            {
                await System.Threading.Tasks.Task.Yield();
            }

            if (RefreshResolversAfterCompletion && SettingsProvider.LastUsedSettingsProvider != null && SettingsProvider.LastUsedSettingsProvider.HasSettings())
            {
                SettingsProvider.LastUsedSettingsProvider.Settings.RefreshRegisteredResolversWithConnection<ResolutionConnection>();
                SettingsProvider.LastUsedSettingsProvider.Settings.RefreshRegisteredResolversWithConnection<RefreshRateConnection>();
            }

            OnComplete?.Invoke();
        }
#endif
    }
}
