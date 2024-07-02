using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class VSyncConnection : Connection<bool>
    {
        public override bool Get()
        {
            return QualitySettings.vSyncCount != 0;
        }

        public override void Set(bool vSyncEnabled)
        {
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
            NotifyListenersIfChanged(vSyncEnabled);

#if UNITY_EDITOR
            if (SettingsGeneratorSettings.GetOrCreateSettings().ShowEditorInfoLogs)
            {
                Logger.LogMessage("Setting V-Sync has no effect in the Editor. Please try in a build. - " + SettingsGeneratorSettings._showEditorInfoLogsHint);
            }
#endif
        }
    }
}
