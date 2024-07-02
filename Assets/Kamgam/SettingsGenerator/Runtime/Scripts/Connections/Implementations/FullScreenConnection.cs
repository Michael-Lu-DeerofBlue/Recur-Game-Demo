using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class FullScreenConnection : Connection<bool>
    {
        protected bool? lastKnownFullScreen = null;
        protected int lastSetFrame = 0;

        public override bool Get()
        {
            bool result;

            // Reset after N frames. The assumption is that
            // after that the Screen.fullScreen has been updated.
            if (Time.frameCount - lastSetFrame > 3)
                lastKnownFullScreen = null;

            if (lastKnownFullScreen.HasValue)
                result = lastKnownFullScreen.Value;
            else
                result = Screen.fullScreen;

            return result;
        }

        /// <summary>
        /// NOTICE: A full screen switch does not happen immediately; it happens when the current frame is finished.
        /// See: https://docs.unity3d.com/ScriptReference/Screen-fullScreen.html
        /// </summary>
        /// <param name="fullScreen">Fullscreen on or off.</param>
        public override void Set(bool fullScreen)
        {
            // Request change but delegate the actual execution to the orchestrator.
            ScreenOrchestrator.Instance.RequestFullScreen(fullScreen);

            // Remember
            lastSetFrame = Time.frameCount;
            lastKnownFullScreen = fullScreen;

            NotifyListenersIfChanged(fullScreen);

#if UNITY_EDITOR
            if (SettingsGeneratorSettings.GetOrCreateSettings().ShowEditorInfoLogs)
            {
                Logger.LogMessage("Setting FullScreen has no effect in the Editor. Please try in a build. - " + SettingsGeneratorSettings._showEditorInfoLogsHint);
            }
#endif
        }
    }
}
