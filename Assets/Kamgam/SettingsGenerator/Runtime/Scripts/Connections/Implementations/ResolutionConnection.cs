using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class ResolutionConnection : ConnectionWithOptions<string>
    {
        /// <summary>
        /// It is not advisable to change the resolution on mobile.
        /// It may have unexpected sideeffect and there usually is
        /// just one anyways.
        /// 
        /// </summary>
        public static bool AllowResolutionChangeOnMobile = false;

        /// <summary>
        /// Disable if the resolutions change very often.
        /// </summary>
        public bool CacheResolutions = true;

        /// <summary>
        /// If enabled then only those resolution options which match the
        /// current resolution refresh rate are listed. That list may be
        /// much shorter than the full list.
        /// </summary>
        public bool LimitToCurrentRefreshRate = false;

        /// <summary>
        /// If enabled then only one resolution per frequency will be listed.<br />
        /// For example there may be two resolutions: 640x480 @60Hz and 640x480 @75Hz<br />
        /// If enabled then only one of these will be in the list. It will choose the one
        /// which has the closest frequency to the currently used frequency.
        /// </summary>
        public bool LimitToUniqueResolutions = true;

        /// <summary>
        /// If enabled then any resolution that is bigger than the width or height of the biggest screen (hardware resolution) will be skipped.<br /><br />
        /// NOTICE: This does nothing in the EDITOR since the API does not return the correct size there. Please test it in a real build.
        /// </summary>
        public bool LimitMaxResolutionToDisplayResolution = false;

        /// <summary>
        /// If enabled then then resolutions with a refresh rate of 59 Hz will be
        /// skipped if (and only if) there is an alternative with 60 Hz.
        /// </summary>
        public bool SkipRefreshRatesWith59Hz = true;

        /// <summary>
        /// Should the refresh rate (frequency) be added to the labels.<br />
        /// Example without: 1024x768<br />
        /// Example with: 1024x768 (60Hz)<br />
        /// </summary>
        public bool AddRefreshRateToLabels = false;

        public event System.Action OnMaxResolutionChanged;

        /// <summary>
        /// A list of aspect ratios (width, height) to use as a positive filter criteria for the list of resolutions.<br />
        /// If the list is empty then no filtering will occur and all resolutions will be listed.<br />
        /// </summary>
        public List<Vector2Int> AllowedAspectRatios = new List<Vector2Int>();

        /// <summary>
        /// Threshold of how much a resolution can differ from the defined AllowedAspectRatios.<br />
        /// Like if the allowed aspect is 16:9 (w:h), i.e.: 1.77 and this is 0.02f then all ratios between 1.75 anf 1.79 are valid. 
        /// </summary>
        public float AllowedAspectRatioDelta = 0.02f;


        protected List<Resolution> _values;
        protected List<string> _labels;

        /// <summary>
        /// Resolution format {0} = width in pixels. {1} = height in pixels.
        /// </summary>
        protected string _resolutionFormat = "{0}x{1}";

        /// <summary>
        /// Will be appended to the normal resolution string if
        /// AddRefreshRateToLabels is enabled. {0} is the refresh
        /// rate as an integer.
        /// </summary>
        protected string _refreshRateFormat = " ({0}Hz)";

        // We use this value to detect whether or not the avilable resolutions have changed.
        // This usually happens if the app has been moved to another monitor.
        protected Vector2Int _lastMonitorMaxResolution;

        protected Vector2Int getCurrentMaxResolution()
        {
            var resolutions = Screen.resolutions;
            return new Vector2Int(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height);
        }

        protected List<Resolution> getUniqueResolutions()
        {
            if (_values == null || _values.Count == 0 || !CacheResolutions)
            {
                _values = new List<Resolution>();

                // Generate a list of resolutions which have the same refresh rate as the current one.
                var resolutions = Screen.resolutions;
                fillResolutionsList(resolutions, limitAspectRatios: true);
                // If no resolutions are found then don't filter.
                if (_values.Count == 0)
                {
                    Logger.LogWarning("Resolution aspect ratio limiting resulted in an empty list. Disabling filtering (all resolutions will be listed).");
                    fillResolutionsList(resolutions, limitAspectRatios: false);
                }

                // Hard fallback
                if (_values.Count == 0)
                {
                    var res = new Resolution();
                    res.width = 1024;
                    res.height = 768;
#if UNITY_2022_2_OR_NEWER
                    var r = new RefreshRate();
                    r.numerator = 60000;
                    r.denominator = 1001;
                    res.refreshRateRatio = r;
#else
                    res.refreshRate = 60;
#endif
                    _values.Add(res);
                }
            }

            return _values;
        }

        private void fillResolutionsList(Resolution[] resolutions, bool limitAspectRatios)
        {
#if UNITY_EDITOR
            // Sadly the Display.systemWidth API does NOT return the native resolution in the editor. Thus we disable this.
            // See: https://forum.unity.com/threads/display-systemwidth-returns-game-view-width-in-editor.1456138/
            LimitMaxResolutionToDisplayResolution = false;
#endif
            float maxSystemWidth = 0f;
            float maxSystemHeight = 0f;
            if (LimitMaxResolutionToDisplayResolution)
            {
                foreach (var display in Display.displays)
                {
                    maxSystemWidth = Mathf.Max(maxSystemWidth, display.systemWidth);
                    maxSystemHeight = Mathf.Max(maxSystemHeight, display.systemHeight);
                }
            }

            foreach (var res in resolutions)
            {
                // Skip 59 Hz resolutions if there is a 60 Hz alternative.
                if (SkipRefreshRatesWith59Hz && !LimitToCurrentRefreshRate && getRoundedRefreshRate(res) == 59)
                {
                    var resWith60Hz = findResolution(resolutions, res.width, res.height, 60);
                    // Found 60 Hz alternative > skip.
                    if (resWith60Hz.HasValue)
                        continue;
                }

                // Limit the options to the same refresh rate:
                // Sometimes the current refresh rate is 59 (actually 59.94) but the reported rate in the other resolutions is 60.
                // To avoid empty resolution lists if LimitToCurrentRefreshRate is on we allow +/-1.
                if (LimitToCurrentRefreshRate && Mathf.Abs(getRoundedRefreshRate(Screen.currentResolution) - getRoundedRefreshRate(res)) > 1)
                    continue;

                if (LimitToUniqueResolutions)
                {
                    // Skip the exact duplicates
                    if (contains(_values, res))
                        continue;
                }

                if (LimitMaxResolutionToDisplayResolution && maxSystemWidth > 0f)
                {
                    if (res.width > maxSystemWidth || res.height > maxSystemHeight)
                        continue;
                }

                // Filter res by aspect ratios
                if (limitAspectRatios && AllowedAspectRatios != null && AllowedAspectRatios.Count > 0)
                {
                    float ratio = (float)res.width / res.height;
                    foreach (var aspect in AllowedAspectRatios)
                    {
                        float allowedRatio = (float)aspect.x / aspect.y;
                        if (Mathf.Abs(ratio - allowedRatio) <= AllowedAspectRatioDelta)
                        {
                            _values.Add(res);
                            break;
                        }
                    }
                }
                else
                {
                    // No filtering
                    _values.Add(res);
                }
            }

            // We have to do the advanced duplicate res check AFTER the list has been filled because
            // otherwise the "Check if there is another one with a smaller delta" would also include
            // resolutions which are already filtered out.
            if (LimitToUniqueResolutions)
            {
                for (int i = _values.Count-1; i >= 0; i--)
                {
                    var res = _values[i];

                    // Skip the duplicate resolutions but keep the one which has the closest refresh rate.
                    //   For example there may be two resolutions: 640x480 @60Hz and 640x480 @75Hz
                    //   If enabled then only one of these will be in the list. It will choose the one
                    //   which has the closest frequency to the currently used frequency.
                    int refreshRateDelta = Mathf.Abs(getRoundedRefreshRate(Screen.currentResolution) - getRoundedRefreshRate(res));
                    // Check if there is another one with a smaller delta
                    int smallerDelta = int.MaxValue;
                    foreach (var r in _values)
                    {
                        if (r.width != res.width || r.height != res.height)
                            continue;

                        smallerDelta = Mathf.Abs(getRoundedRefreshRate(Screen.currentResolution) - getRoundedRefreshRate(r));
                        if (smallerDelta < refreshRateDelta)
                            break;
                    }
                    // Skip if other resolution with smaller refresh rate delta was found.
                    if (smallerDelta < refreshRateDelta)
                    {
                        _values.RemoveAt(i);
                        continue;
                    }
                }
                
            }
        }

        protected Resolution? findResolution(Resolution[] resolutions, int width, int height, int refreshRate)
        {
            foreach (var res in resolutions)
            {
                // Limit the options to the same refresh rate.
                int roundedRefreshRate = getRoundedRefreshRate(res);
                if (res.width == width && res.height == height && roundedRefreshRate == refreshRate)
                {
                    return res;
                }
            }

            return null;
        }

        protected int getRoundedRefreshRate(Resolution res)
        {
#if UNITY_2022_2_OR_NEWER
            return Mathf.RoundToInt((float)res.refreshRateRatio.value);
#else
            return res.refreshRate;
#endif
        }

        protected bool contains(List<Resolution> resolutions, Resolution resolution)
        {
            if (resolutions == null || resolutions.Count == 0)
                return false;

            for (int i = 0; i < resolutions.Count; i++)
            {
                // Interpret 59 Hz and 60 Hz as the same.
                bool rateIsSimilar = Mathf.Abs(getRoundedRefreshRate(resolutions[i]) - getRoundedRefreshRate(resolution)) <= 1;
                if (resolution.width == resolutions[i].width && resolution.height == resolutions[i].height && rateIsSimilar)
                    return true;
            }

            return false;
        }

        public override List<string> GetOptionLabels()
        {
            // Reset values and labels if monitor max resolution changed.
            var maxResolution = getCurrentMaxResolution();
            if (maxResolution != _lastMonitorMaxResolution)
            {
                _lastMonitorMaxResolution = maxResolution;

                _values = null;
                _labels = null;

                OnMaxResolutionChanged?.Invoke();
            }

            if (_labels == null || _labels.Count == 0 || !CacheResolutions)
            {
                _labels = new List<string>();

                var resolutions = getUniqueResolutions();
                foreach (var res in resolutions)
                {
                    string name = string.Format(_resolutionFormat, res.width, res.height);
                    if (AddRefreshRateToLabels)
                    {
                        name += string.Format(_refreshRateFormat, getRoundedRefreshRate(res));
                    }
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
            var resolutions = getUniqueResolutions();
            if (optionLabels == null || optionLabels.Count != resolutions.Count)
            {
                Logger.LogError("Invalid new labels. Need to be " + resolutions.Count + ".");
            }

            _labels = new List<string>(optionLabels);
        }

        public string GetResolutionFormat()
        {
            return _resolutionFormat;
        }

        public void SetResolutionFormat(string format)
        {
            _resolutionFormat = format;
            RefreshOptionLabels();
        }

        public string GetRefreshRateFormat()
        {
            return _refreshRateFormat;
        }

        public void SetRefreshRateFormat(string format)
        {
            _refreshRateFormat = format;
            RefreshOptionLabels();
        }

        protected Resolution? lastKnownResolution = null;
        protected int lastSetFrame = 0;

        public override int Get()
        {
            // Reset after N frames. The assumption is that
            // after that the Screen.currentResolution has been updated.
            if (Time.frameCount - lastSetFrame > 3)
                lastKnownResolution = null;

            Resolution currentResolution = Screen.currentResolution;
            if (lastKnownResolution.HasValue)
                currentResolution = lastKnownResolution.Value;

            // Find the closest resolution. Usually they match exactly but after
            // a monitor changed they may not so we search for the best match.
            var resolutions = getUniqueResolutions();
            int minDelta = int.MaxValue;
            int closestResolutionIndex = 0;
            for (int i = 0; i < resolutions.Count; i++)
            {
                int delta = Mathf.Abs(resolutions[i].width - currentResolution.width) + Mathf.Abs(resolutions[i].height - currentResolution.height);
                if (delta < minDelta)
                {
                    minDelta = delta;
                    closestResolutionIndex = i;

                    // Shortcut
                    if (minDelta == 0)
                        return i;
                }
            }

            return closestResolutionIndex;
        }

        /// <summary>
        /// NOTICE: This has no effect in the Edtior.<br />
        /// NOTICE: A resolution switch does not happen immediately; it happens when the current frame is finished.<br />
        /// See: https://docs.unity3d.com/ScriptReference/Screen.SetResolution.html
        /// </summary>
        /// <param name="index"></param>
        public override void Set(int index)
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_SWITCH
            if (!AllowResolutionChangeOnMobile)
            {
                Logger.LogWarning("Allow resolution change on mobile is disabled. It is not advisable to change the resolution on mobile. It may have unexpected sideeffects and there usually is just one anyways. If you are on URP then use the renderScale instead.");
                return;
            }
#endif

            var resolutions = getUniqueResolutions();
            index = Mathf.Clamp(index, 0, Mathf.Max(0, resolutions.Count - 1));
            var resolution = resolutions[index];

            // Request change but delegate the actual execution to the orchestrator.
            ScreenOrchestrator.Instance.RequestResolution(resolution);

            // remember
            lastSetFrame = Time.frameCount;
            lastKnownResolution = resolution;

            NotifyListenersIfChanged(index);
        }
    }
}
