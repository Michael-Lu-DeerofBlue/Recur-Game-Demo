using System.Collections;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// All the screen related functions are async (executed at the end of the frame)<br />
    /// and some are contradicting each other, for example:
    /// Screen.fullScreen = true VS Screen.fullScreenMode = FullScreenMode.Windowed
    /// 
    /// To resove this it was decided that Screen.fullScreenMode always takes precedence.
    /// 
    /// There fore we need this helper to execute them properly in order.
    /// </summary>
    public class ScreenOrchestrator : MonoBehaviour
    {
        private static ScreenOrchestrator _instance;
        public static ScreenOrchestrator Instance
        {
            get
            {
                if (!_instance)
                {
#if UNITY_EDITOR
                    // Keep the instance null outside of play mode to avoid leaking
                    // instances into the scene.
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        return null;
                    }
#endif
                    _instance = new GameObject().AddComponent<ScreenOrchestrator>();
                    _instance.name = _instance.GetType().ToString();
#if UNITY_EDITOR
                    _instance.hideFlags = HideFlags.DontSave;
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
#endif
                        DontDestroyOnLoad(_instance.gameObject);
#if UNITY_EDITOR
                    }
#endif
                }
                return _instance;
            }
        }

        protected Resolution? requestedResolution;
// Notice: The RefreshRate class has been added in 2021_2 (yes 2021) BUT it is used in Screen.SetResolution only in 2022.2 (yes 2022, not 2021)
#if UNITY_2022_2_OR_NEWER
        protected RefreshRate? requestedRefreshRate;
#else
        protected int? requestedRefreshRate;
#endif

        protected bool? requestedFullScreen;
        protected FullScreenMode? requestedFullScreenMode;

        protected Coroutine _applyCoroutine;

        public void RequestResolution(Resolution resolution)
        {
            requestedResolution = resolution;
        }

// Notice: The RefreshRate class has been added in 2021_2 (yes 2021) BUT it is used in Screen.SetResolution only in 2022.2 (yes 2022, not 2021)
#if UNITY_2022_2_OR_NEWER
        public void RequestRefreshRate(RefreshRate refreshRate)
        {
            requestedRefreshRate = refreshRate;
        }
#else
        public void RequestRefreshRate(int refreshRate)
        {
            requestedRefreshRate = refreshRate;
        }
#endif

        public void RequestFullScreen(bool fullScreen)
        {
            requestedFullScreen = fullScreen;
        }

        public void RequestFullScreenMode(FullScreenMode fullScreenMode)
        {
            requestedFullScreenMode = fullScreenMode;
        }

        public void LateUpdate()
        {
            if (requestedResolution.HasValue || requestedFullScreen.HasValue || requestedFullScreenMode.HasValue)
            {
                apply();
            }
        }

        protected void apply()
        {
            if (_applyCoroutine != null)
            {
                StopCoroutine(_applyCoroutine);
            }

            _applyCoroutine = StartCoroutine(applyStaggered());
        }

        protected IEnumerator applyStaggered()
        {
            // Copy
            var tRequestedFullScreen = requestedFullScreen;
            var tRequestedFullScreenMode = requestedFullScreenMode;
            var tRequestedResolution = requestedResolution;
            var tRequestedRefreshRate = requestedRefreshRate;

            // Reset immediately
            requestedFullScreen = null;
            requestedFullScreenMode = null;
            requestedResolution = null;
            requestedRefreshRate = null;

            if (tRequestedFullScreen.HasValue)
            {
                if (!tRequestedFullScreen.Value)
                {
                    Screen.fullScreen = false;
                }
                else
                {
                    Screen.fullScreen = true;
                }

                // Wait one frame
                yield return null;
            }

            if (tRequestedFullScreenMode.HasValue)
            {
                Screen.fullScreenMode = tRequestedFullScreenMode.Value;

                // Wait one frame
                yield return null;
            }

            if (tRequestedResolution.HasValue)
            {
                var resolution = tRequestedResolution.Value;
// Notice: The RefreshRate class has been added in 2021_2 (yes 2021) BUT it is used in Screen.SetResolution only in 2022.2 (2022, not 2021)
#if UNITY_2022_2_OR_NEWER
                var refreshRate = tRequestedRefreshRate.HasValue ? tRequestedRefreshRate.Value : tRequestedResolution.Value.refreshRateRatio;
#else
                var refreshRate = tRequestedRefreshRate.HasValue ? tRequestedRefreshRate.Value : tRequestedResolution.Value.refreshRate;
#endif
                var fullScreenMode = tRequestedFullScreenMode.HasValue ? tRequestedFullScreenMode.Value : Screen.fullScreenMode;
                Screen.SetResolution(resolution.width, resolution.height, fullScreenMode, refreshRate);
            }
            else if (tRequestedRefreshRate.HasValue)
            {
                var res = Screen.currentResolution;
                var fullScreenMode = tRequestedFullScreenMode.HasValue ? tRequestedFullScreenMode.Value : Screen.fullScreenMode;
                Screen.SetResolution(res.width, res.height, fullScreenMode, tRequestedRefreshRate.Value);
            }
        }

        public void Destroy()
        {
            _instance = null;

            if (this != null && this.gameObject != null)
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    DestroyImmediate(this.gameObject);
                }
#else
                Destroy(this.gameObject);
#endif
            }
        }
    }
}
